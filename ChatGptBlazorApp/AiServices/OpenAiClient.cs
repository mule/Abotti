using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace ChatGptBlazorApp.AiServices;

public class OpenAiClient : IOpenAiClient
{
    private readonly IOpenAIService _openAiService;

    public OpenAiClient(IOpenAIService openAiService)
    {
        _openAiService = openAiService;
    }

    public async Task<string> GetCompletion(string prompt)
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

        if (completion.Choices.Any())
            return completion.Choices.First().Message.Content;
        return "No response";
    }
}

public interface IOpenAiClient
{
    Task<string> GetCompletion(string prompt);
}