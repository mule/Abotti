namespace ChatGptBlazorCore.Models;

public record ChatEntry
{
    public string? Role { get; set; }
    public string? Content { get; set; }
}