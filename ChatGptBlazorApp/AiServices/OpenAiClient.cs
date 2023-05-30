using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace ChatGptBlazorApp.AiServices;

public class OpenAiClient : IOpenAiClient
{
    private readonly ILogger<OpenAiClient> _logger;
    private readonly IOpenAIService _openAiService;

    public OpenAiClient(IOpenAIService openAiService, ILogger<OpenAiClient> logger)
    {
        _openAiService = openAiService;
        _logger = logger;
    }

    public async Task<(bool Success, string Response, string[] Errors)> GetCompletion(string prompt)
    {
        var request = new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                new("assistant", "You are a helpful assistant."),
                new("user", prompt)
            },
            Model = "gpt-3.5-turbo"
        };
        var completion = await _openAiService.ChatCompletion.CreateCompletion(request);

        if (completion == null) _logger.LogError("Empty response from OpenAI");

        if (!completion.Successful)
        {
            _logger.LogError("Errors from OpenAI: {Errors}", completion.Error?.Message);
            return (false, "", new[] { completion.Error?.Message ?? "Unknown error" });
        }


        if (completion.Choices.Any())
            return (true, completion.Choices.First().Message.Content, Array.Empty<string>());
        return (true, "", Array.Empty<string>());
    }
}

public interface IOpenAiClient
{
    Task<(bool Success, string Response, string[] Errors)> GetCompletion(string prompt);
}