using Microsoft.Extensions.Logging;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;

namespace ServiceAccessLayer.AiServices;

public class OpenAiClient : IOpenAiClient
{
    private readonly string _llmModel;
    private readonly ILogger<OpenAiClient> _logger;

    private readonly IOpenAIService _openAiService;

    public OpenAiClient(IOpenAIService openAiService, ILogger<OpenAiClient> logger, string llmModel)
    {
        _openAiService = openAiService;
        _logger = logger;
        _llmModel = llmModel;
    }


    public async Task<(bool Success, string Response, string[] Errors)> GetCompletion(string prompt)
    {
        var result = await GetCompletion(new List<(string Role, string Message)>(), prompt);
        return result;
    }

    public async Task<(bool Success, string Response, string[] Errors)> GetCompletion(
        IEnumerable<(string Role, string Message)> previousMessages, string prompt)
    {
        var messages = new List<ChatMessage>();

        if (previousMessages.Any())
        {
            messages.Add(new ChatMessage("system",
                "You are a helpful assistant and you have been having the following conversation with a user:"));
            messages.AddRange(previousMessages.Select(m => new ChatMessage(m.Role, m.Message)));
            messages.Add(new ChatMessage("system", "The user has sent you the following message:"));
            messages.Add(new ChatMessage("user", prompt));
        }
        else
        {
            messages = new List<ChatMessage>
            {
                new("system",
                    "You are a helpful assistant and the user has initiated a conversation with you with the following message:"),
                new("user", prompt)
            };
        }

        var request = new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = _llmModel
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
            Model = _llmModel
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

    Task<(bool Success, string Response, string[] Errors)> GetCompletion(
        IEnumerable<(string Role, string Message)> previousMessages, string prompt);

    Task<(bool Success, string Response, string[] Errors)> GetChatTopic((string Role, string Message)[] messages);
}