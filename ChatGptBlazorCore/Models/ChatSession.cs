namespace ChatGptBlazorCore.Models;

public class ChatSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string Topic { get; set; }

    public DateTime Created { get; set; }

    public string RootPrompt { get; set; }


    public static ChatSession GenerateTestChatSession(Guid userId)
    {
        var id = Guid.NewGuid();
        var idString = id.ToString();
        //get last 5 characters from id
        idString = idString.Substring(idString.Length - 5);
        return new ChatSession
        {
            Id = id,
            UserId = userId,
            Topic = $"Test topic {idString}",
            Created = DateTime.Now,
            RootPrompt = $"Test Prompt {idString}"
        };
    }
}