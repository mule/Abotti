// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenAI.Extensions;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using Serilog;
using Serilog.Debugging;
using ServiceAccessLayer.AiServices;
using Spectre.Console;

static void WriteDivider(string text)
{
    AnsiConsole.WriteLine();
    AnsiConsole.Write(new Rule($"[yellow]{text}[/]").RuleStyle("grey").LeftJustified());
}


var sessionMessages = new List<(string Role, string Message)>();

try
{
    SelfLog.Enable(msg => Debug.WriteLine(msg));
    SelfLog.Enable(Console.Error);

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        // .WriteTo.SpectreConsole("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}")   
        .WriteTo.Console()
        .CreateLogger();
    var config = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();
    AnsiConsole.WriteLine("Welcome to OpenAi development console");

    var services = new ServiceCollection()
        .AddLogging()
        .AddOpenAIService()
        .Services.AddSingleton<IConfiguration>(config)
        .AddTransient(provider => new OpenAiClient(
            provider.GetService<IOpenAIService>(),
            provider.GetService<ILogger<OpenAiClient>>(), Models.Gpt_3_5_Turbo));

    var serviceProvider = services.BuildServiceProvider();

    var openAiClient = serviceProvider.GetService<OpenAiClient>();


    var prompt = AnsiConsole.Ask<string>("Enter your prompt (or press enter 'quit' for exit):");

    sessionMessages.Add(("user", prompt));


    while (prompt.Trim().ToLower() != "quit")
    {
        await AnsiConsole.Status().StartAsync("Development console: Waiting for response...", async context =>
        {
            var response = await openAiClient.GetCompletion(sessionMessages, prompt);
            var chatTopic = await openAiClient.GetChatTopic(sessionMessages.ToArray());
            context.Status("Development console: Response received");
            context.Refresh();

            if (response.Success)
            {
                sessionMessages.Add(("assistant", response.Response));
                try
                {
                    AnsiConsole.MarkupLine($"OpenAI ({chatTopic.Response}): {response.Response}");
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, "Unable to write response as markup");
                    AnsiConsole.WriteLine($"OpenAI ({chatTopic.Response}): {response.Response}");
                }
            }
            else
            {
                foreach (var error in response.Errors) Log.Logger.Error(error);
            }
        });

        prompt = AnsiConsole.Ask<string>("\nYou: ");
        sessionMessages.Add(("user", prompt));
    }

    AnsiConsole.WriteLine("Goodbye!");
}
catch (Exception e)
{
    Log.Logger.Fatal(e, "Unrecoverable error");
    throw;
}
finally
{
    Log.CloseAndFlush();
}