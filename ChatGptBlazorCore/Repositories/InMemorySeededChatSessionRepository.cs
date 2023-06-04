namespace ChatGptBlazorCore.Models;

public class InMemorySeededChatSessionRepository : IChatSessionRepository
{
    private readonly Dictionary<Guid, ChatSession> _chatSessions = new();

    public InMemorySeededChatSessionRepository()
    {
    }


    public InMemorySeededChatSessionRepository(Dictionary<Guid, ChatSession> chatSessions)
    {
        _chatSessions = chatSessions;
    }

    public ChatSession[] GetChatSessions(Guid userId)
    {
        return _chatSessions.Values.Where(x => x.UserId == userId).ToArray();
    }

    public (Guid Id, string Topic, DateTime Created)[] GetUserChatSessionTopics(Guid userId)
    {
        return _chatSessions.Values.Where(x => x.UserId == userId).Select(x => (x.Id, x.Topic, x.Created)).ToArray();
    }

    public ChatSession? GetChatSession(Guid chatSessionId)
    {
        return _chatSessions.TryGetValue(chatSessionId, out var chatSession) ? chatSession : null;
    }

    public ChatSession? AddChatSession(Guid userId, string rootPrompt)
    {
        var chatSession = new ChatSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RootPrompt = rootPrompt
        };
        _chatSessions.Add(chatSession.Id, chatSession);
        return chatSession;
    }

    public (bool Ok, string[] Errors) AddChatSession(ChatSession chatSession)
    {
        if (_chatSessions.ContainsKey(chatSession.Id))
            return (false, new[] { "Chat session already exists" });
        _chatSessions.Add(chatSession.Id, chatSession);
        return (true, Array.Empty<string>());
    }


    public (bool Ok, string[] Errors) DeleteChatSession(Guid chatSessionId)
    {
        if (_chatSessions.Remove(chatSessionId))
            return (true, Array.Empty<string>());
        return (false, new[] { "Chat session not found" });
    }

    public (bool Ok, string[] Errors) DeleteChatSessions(Guid userId)
    {
        var chatSessions = _chatSessions.Values.Where(x => x.UserId == userId).ToArray();
        foreach (var chatSession in chatSessions) _chatSessions.Remove(chatSession.Id);

        return (true, Array.Empty<string>());
    }

    public (bool Ok, ChatSession Result, string[] Errors) UpdateChatSession(Guid chatSessionId, string rootPrompt)
    {
        if (!_chatSessions.TryGetValue(chatSessionId, out var chatSession))
            return (false, null!, new[] { "Chat session not found" });
        chatSession.RootPrompt = rootPrompt;
        return (true, chatSession, Array.Empty<string>());
    }
}