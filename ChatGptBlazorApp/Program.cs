using Blazored.Toast;
using ChatGptBlazorApp.Areas.Identity.Data;
using ChatGptBlazorCore.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using OpenAI.GPT3.Extensions;
using Serilog;
using Serilog.Sinks.SpectreConsole;
using ServiceAccessLayer.AiServices;

// Configure Serilog logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.SpectreConsole("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var connectionString = builder.Configuration.GetConnectionString("ChatGptBlazorAppContextConnection") ??
                           throw new InvalidOperationException(
                               "Connection string 'ChatGptBlazorAppContextConnection' not found.");
    var config = builder.Configuration;

    var adminUserIdStr = builder.Configuration.GetValue<string>("AdminUser:Id");
    var adminUserId = Guid.Parse(adminUserIdStr);
    var adminUserName = builder.Configuration.GetValue<string>("AdminUser:UserName");

    var testChatRepo = new InMemorySeededChatSessionRepository();
    testChatRepo.AddChatSession(ChatSession.GenerateTestChatSession(adminUserId));
    testChatRepo.AddChatSession(ChatSession.GenerateTestChatSession(adminUserId));
    testChatRepo.AddChatSession(ChatSession.GenerateTestChatSession(adminUserId));


    builder.Host.UseSerilog();
    builder.Services.AddDbContext<ChatGptBlazorAppContext>(options => options.UseSqlServer(connectionString));

    builder.Services.AddDefaultIdentity<ChatGptBlazorAppUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ChatGptBlazorAppContext>();

    builder.Configuration.AddUserSecrets<Program>();

    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
    builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);
    builder.Services.AddMicrosoftIdentityConsentHandler();


// Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddOpenAIService();
    builder.Services.AddScoped<OpenAiClient>();
    builder.Services.AddBlazoredToast();
    builder.Services.AddControllersWithViews(options =>
    {
        // var policy = new AuthorizationPolicyBuilder()
        //     .RequireAuthenticatedUser()
        //     .Build();
        // options.Filters.Add(new AuthorizeFilter(policy));
    }).AddMicrosoftIdentityUI();

    builder.Services.AddAuthorization(options =>
    {
        // By default, all incoming requests will be authorized according to the default policy
        options.FallbackPolicy = options.DefaultPolicy;
    });

    builder.Services.AddSingleton<IUserRepository, InMemorySeededUserRepository>(ctx =>
        new InMemorySeededUserRepository(adminUserId, adminUserName));


    builder.Services.AddSingleton<IChatSessionRepository, InMemorySeededChatSessionRepository>(ctx => testChatRepo);

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


    app.UseStaticFiles();

    app.UseRouting();
    app.UseHttpsRedirection();

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();
}
catch (Exception e)
{
    Log.Fatal("Application start-up failed", e);
}
finally
{
    Log.CloseAndFlush();
}


public class AssemblyLocator
{
};