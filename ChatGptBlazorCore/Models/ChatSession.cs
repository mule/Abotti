namespace ChatGptBlazorCore.Models;

public class ChatSession
{
    public ChatSession(Guid userId)
    {
        Id = Guid.NewGuid();
        Created = DateTime.Now;
        UserId = userId;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string Topic { get; set; }

    public List<string> Tags { get; set; }

    public DateTime Created { get; set; }

    public List<ChatEntry> Entries { get; set; }


    public static ChatSession GenerateTestChatSession(Guid userId)
    {
        var id = Guid.NewGuid();
        var idString = id.ToString();
        //get last 5 characters from id
        idString = idString.Substring(idString.Length - 5);
        var entries = new List<ChatEntry>();

        //generate 10 entries
        for (var i = 0; i < 10; i++)
            entries.Add(new ChatEntry
            {
                Role = i % 2 == 0 ? "User" : "Assistant",
                Content = $"Test entry {i} for chat session {idString}"
            });

        return new ChatSession(userId)
        {
            Topic = $"Test topic {idString}",
            Entries = entries
        };
    }
}