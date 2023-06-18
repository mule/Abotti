using Microsoft.Extensions.Logging;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace ServiceAccessLayer.AiServices;

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

        if (completion?.Successful is not true)
        {
            _logger.LogError("Errors from OpenAI: {Errors}", completion?.Error?.Message);
            return (false, "", new[] { completion.Error?.Message ?? "Unknown error" });
        }


        if (completion.Choices.Any())
            return (true, completion.Choices.First().Message.Content, Array.Empty<string>());
        return (true, "", Array.Empty<string>());
    }

    // Try to get chet session topic from openAI by giving it prompts and responses
    public async Task<(bool Success, string Response, string[] Errors)> GetChatTopic(
        (string Role, string Message)[] messages)
    {
        var request = new ChatCompletionCreateRequest
        {
            Messages = messages.Select(x => new ChatMessage(x.Role, x.Message)).ToList(),
            Model = "gpt-3.5-turbo"
        };
        var completion = await _openAiService.ChatCompletion.CreateCompletion(request);

        if (completion == null) _logger.LogError("Empty response from OpenAI");

        if (completion?.Successful is not true)
        {
            _logger.LogError("Errors from OpenAI: {Errors}", completion?.Error?.Message);
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
    Task<(bool Success, string Response, string[] Errors)> GetChatTopic((string Role, string Message)[] messages);
}