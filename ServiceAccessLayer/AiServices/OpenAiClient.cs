using System.Text;
using Microsoft.Extensions.Logging;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;

namespace ServiceAccessLayer.AiServices;

public class OpenAiClient : IOpenAiClient
{
    private readonly ChatMessage[] _chatContinueDefaultAppendedMessages =
    {
        ChatMessage.FromSystem("The user has sent you the following new message:")
    };

    private readonly ChatMessage[] _chatContinueDefaultPrependedMessages =
    {
        ChatMessage.FromSystem(
            "You are a helpful assistant and you have been having the following conversation with a user:")
    };

    private readonly ChatMessage[] _chatStartDefaultPrependedMessages =
    {
        ChatMessage.FromSystem(
            "You are a helpful assistant and the user has initiated a conversation with you with the following message:")
    };

    private readonly ChatMessage[] _classificationDefaultAppendedMessages =
    {
        ChatMessage.FromSystem("Give your answer in this form {main topic};{sub topic};{sub topic}"),
        ChatMessage.FromSystem(
            "It is very important to you limit each response to maximum of 3 words and maximum of 3 topics")
    };

    private readonly ChatMessage[] _classificationDefaultPrependedMessages =
    {
        ChatMessage.FromSystem("You are a language model that helps classify discussions."),
        ChatMessage.FromSystem("The user has sent you the following message:")
    };

    private readonly string _llmModel;
    private readonly ILogger<OpenAiClient> _logger;
    private readonly IOpenAIService _openAiService;


    public OpenAiClient(IOpenAIService openAiService, ILogger<OpenAiClient> logger, string llmModel = "gpt-3.5-turbo")
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
            messages.AddRange(_chatContinueDefaultPrependedMessages);
            messages.AddRange(previousMessages.Select(m => new ChatMessage(m.Role, m.Message)));
            messages.AddRange(_chatContinueDefaultAppendedMessages);
            messages.Add(ChatMessage.FromUser(prompt));
        }
        else
        {
            messages.AddRange(_chatStartDefaultPrependedMessages);
            messages.Add(ChatMessage.FromUser(prompt));
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
        (string Role, string Message)[] chatMessages)
    {
        var messages = new List<ChatMessage>();
        messages.AddRange(_classificationDefaultPrependedMessages);
        //messages.AddRange(chatMessages.Select(m => new ChatMessage(m.Role, m.Message)));

        var stringBuilder = new StringBuilder();
        stringBuilder.Append("[");
        foreach (var chatMessage in chatMessages)
            stringBuilder.Append($"{{\"role\": \"{chatMessage.Role}\", \"content\": \"{chatMessage.Message}\"}},");
        stringBuilder.Append("]");

        var conversation = stringBuilder.ToString();
        messages.Add(ChatMessage.FromUser(conversation));
        messages.AddRange(_classificationDefaultAppendedMessages);

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
}

public interface IOpenAiClient
{
    Task<(bool Success, string Response, string[] Errors)> GetCompletion(string prompt);

    Task<(bool Success, string Response, string[] Errors)> GetCompletion(
        IEnumerable<(string Role, string Message)> previousMessages, string prompt);

    Task<(bool Success, string Response, string[] Errors)> GetChatTopic((string Role, string Message)[] messages);
}