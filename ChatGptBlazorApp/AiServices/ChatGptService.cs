using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.AiServices;

/// <summary>
/// Service to access ChatGpt API
/// </summary>
public class ChatGptService
{
    

    private readonly string? _apiKey;

    public ChatGptService(string? secretApiKey = null)
    {
        _apiKey = secretApiKey;
    }
    
    
    public Task<string> GetResponseAsync(string? message)
    {
        return Task.FromResult($"Echo: {message}");
    }

}