using System.Globalization;
using Abotti.BlazorApp.Areas.Identity.Data;
using Abotti.BlazorApp.Localization;
using Abotti.Core.Models;
using Abotti.Core.Repositories;
using Abotti.DataAccessLayer.Repositories;
using Abotti.ServiceAccessLayer.AiServices;
using Azure.Identity;
using Azure.Storage.Blobs;
using Blazored.Toast;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using OpenAI.Extensions;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using Serilog;


// Configure Serilog logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    //.WriteTo.SpectreConsole("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


try
{
    var builder = WebApplication.CreateBuilder(args);


    builder.Configuration.AddEnvironmentVariables();

    Log.Information("Environment is {Environment}", builder.Environment.EnvironmentName);

    builder.Configuration.AddUserSecrets<Program>();


    var azureKeyVaultUriStr = builder.Configuration.GetValue<string>("AzureKeyVaultUri");
    if (!string.IsNullOrEmpty(azureKeyVaultUriStr))
    {
        Log.Information("Adding Azure Key Vault to configuration");
        var azureKeyVaultUri = new Uri(azureKeyVaultUriStr);

        builder.Configuration.AddAzureKeyVault(azureKeyVaultUri, new DefaultAzureCredential());
    }


    var openAiKey = builder.Configuration["OpenAIServiceOptions:ApiKey"];
    var blobStorageConnectionString = builder.Configuration.GetConnectionString("BlobStorage");

    var blobStorageContainerName = builder.Configuration["BlobStorage:ContainerName"];

    var connectionString = builder.Configuration.GetConnectionString("BlazorAppContextConnection") ??
                           throw new InvalidOperationException(
                               "Connection string 'BlazorAppContextConnection' not found.");
    var config = builder.Configuration;


    var adminUserIdStr = builder.Configuration.GetValue<string>("AdminUser:Id");
    var adminUserId = Guid.Parse(adminUserIdStr);
    var adminUserName = builder.Configuration.GetValue<string>("AdminUser:UserName");
    var dataFilesPath = builder.Configuration.GetValue<string>("DataFilesPath");

    if (!Directory.Exists(dataFilesPath))
        Directory.CreateDirectory(dataFilesPath);

    builder.Host.UseSerilog();

    builder.Services.AddDbContext<BlazorAppContext>(options => options.UseSqlServer(connectionString));

    builder.Services.AddDefaultIdentity<BlazorAppUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<BlazorAppContext>();


    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));


// Add services to the container.


    builder.Services.AddLocalization();


    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor()
        .AddMicrosoftIdentityConsentHandler();
    builder.Services.AddOpenAIService(options => { options.ApiKey = openAiKey; });
    builder.Services.AddTransient<IOpenAiClient>(provider => new OpenAiClient(
        provider.GetService<IOpenAIService>(),
        provider.GetService<ILogger<OpenAiClient>>(), Models.Gpt_4));
    builder.Services.AddBlazoredToast();
    builder.Services.AddControllersWithViews(options => { }).AddMicrosoftIdentityUI();

    builder.Services.AddAuthorization(options =>
    {
        // By default, all incoming requests will be authorized according to the default policy
        options.FallbackPolicy = options.DefaultPolicy;
    });

    var blobContainerClient = new BlobContainerClient(blobStorageConnectionString, blobStorageContainerName);
    await blobContainerClient.CreateIfNotExistsAsync();
    var userRepo = new UserBlobStorageRepo(blobContainerClient.GetBlobClient("users.json"), Log.Logger);
    await userRepo.InitializeAsync(new Dictionary<Guid, User>
    {
        { adminUserId, new User(adminUserId, adminUserName, "root") }
    });


    builder.Services.AddSingleton<IUserRepository, UserBlobStorageRepo>(ctx => userRepo);

    var chatSessionRepo =
        new ChatSessionBlobStorageRepository(blobContainerClient.GetBlobClient("chatSessions.json"), Log.Logger);
    await chatSessionRepo.InitializeAsync();

    // var chatSessionRepo = new ChatSessionFileDb(new FileSystem(), Log.Logger, "./Data/chatsessionsdb.json");
    // await chatSessionRepo.InitializeAsync();


    builder.Services.AddSingleton<IChatSessionRepository, ChatSessionBlobStorageRepository>(ctx => chatSessionRepo);

    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.None,
            Secure = CookieSecurePolicy.None
        });
        app.UseHsts();
    }

    var supportedCultures = new[] { "en-US", "fi-FI" };
    var cultureProvider = new RepositoryCultureProvider(userRepo, new CultureInfo("en-US"), Log.Logger);
    var localizationOptions = new RequestLocalizationOptions()
        .SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);

    localizationOptions.RequestCultureProviders.Add(cultureProvider);
    localizationOptions.AddInitialRequestCultureProvider(cultureProvider);


    localizationOptions.FallBackToParentCultures = true;


    app.UseRequestLocalization(localizationOptions);
    app.UseStaticFiles();
    app.UseRouting();
    app.UseHttpsRedirection();
    app.MapControllers();
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Application start-up failed {EMessage} ", e.Message);
}
finally
{
    Log.CloseAndFlush();
}


public class AssemblyLocator
{
}