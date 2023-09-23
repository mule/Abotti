namespace Abotti.Core.Models;

public class ChatSession : IModel<Guid>
{
    public ChatSession()
    {
    }

    public ChatSession(Guid id, Guid userId)
    {
        Id = id;
        Created = DateTime.Now;
        UserId = userId;
        Topic = string.Empty;
        Entries = new List<ChatEntry>();
        Tags = new List<string>();
    }

    public ChatSession(Guid userId) : this(Guid.NewGuid(), userId)
    {
    }

    public Guid UserId { get; set; }

    public string Topic { get; set; }

    public List<string> Tags { get; set; }

    public DateTime Created { get; set; }

    public List<ChatEntry> Entries { get; set; }

    public Guid Id { get; set; }


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

        //generate 3 tags


        return new ChatSession(userId)
        {
            Entries = entries,
            Topic = $"Test topic for chat session {idString}",
            Tags = new List<string>
            {
                "Test tag 1",
                "Test tag 2",
                "Test tag 3"
            }
        };
    }
}