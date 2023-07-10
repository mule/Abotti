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

    public bool HasChatSessions(Guid userId)
    {
        return _chatSessions.Values.Any(x => x.UserId == userId);
    }

    public async Task<bool> HasChatSessionsAsync(Guid userId)
    {
        return await Task.FromResult(_chatSessions.Values.Any(x => x.UserId == userId));
    }

    public async Task<ChatSession[]> GetChatSessionsAsync(Guid userId)
    {
        return await Task.FromResult(_chatSessions.Values.Where(x => x.UserId == userId).ToArray());
    }

    public ChatSession[] GetChatSessions(Guid userId)
    {
        return _chatSessions.Values.Where(x => x.UserId == userId).ToArray();
    }


    public async Task<(Guid Id, string Topic, DateTime Created)[]> GetUserChatSessionTopicsAsync(Guid userId)
    {
        return await Task.FromResult(_chatSessions.Values.Where(x => x.UserId == userId)
            .Select(x => (x.Id, x.Topic, x.Created)).ToArray());
    }

    public (Guid Id, string Topic, DateTime Created)[] GetUserChatSessionTopics(Guid userId)
    {
        return _chatSessions.Values.Where(x => x.UserId == userId).Select(x => (x.Id, x.Topic, x.Created)).ToArray();
    }


    public async Task<ChatSession?> GetChatSessionAsync(Guid chatSessionId, Guid userId)
    {
        return await Task.FromResult(GetChatSession(chatSessionId, userId));
    }

    public ChatSession? GetChatSession(Guid chatSessionId, Guid userId)
    {
        return _chatSessions.TryGetValue(chatSessionId, out var chatSession) && chatSession.UserId == userId
            ? chatSession
            : null;
    }

    public ChatSession? AddChatSession(Guid userId, IEnumerable<ChatEntry> entries = null, string topic = null,
        List<string> tags = null)
    {
        throw new NotImplementedException();
    }


    public async Task<(bool Ok, string[] Errors)> AddChatSessionAsync(ChatSession chatSession)
    {
        var result = await Task.FromResult(AddChatSession(chatSession));
        return result;
    }

    public (bool Ok, string[] Errors) AddChatSession(ChatSession chatSession)
    {
        if (_chatSessions.ContainsKey(chatSession.Id))
            return (false, new[] { "Chat session already exists" });
        _chatSessions.Add(chatSession.Id, chatSession);
        return (true, Array.Empty<string>());
    }

    public async Task<(bool Ok, string[] Errors)> DeleteChatSessionAsync(Guid chatSessionId)
    {
        var result = await Task.FromResult(DeleteChatSession(chatSessionId));
        return result;
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

    public async Task<(bool Ok, ChatSession Result, string[] Errors)> UpdateChatSessionAsync(ChatSession chatSession)
    {
        var result = await Task.FromResult(UpdateChatSession(chatSession));
        return result;
    }

    public (bool Ok, ChatSession Result, string[] Errors) UpdateChatSession(ChatSession chatSession)
    {
        if (!_chatSessions.ContainsKey(chatSession.Id))
            return (false, null!, new[] { "Chat session not found" });
        _chatSessions[chatSession.Id] = chatSession;
        return (true, chatSession, Array.Empty<string>());
    }

    public ChatSession? GetChatSession(Guid chatSessionId)
    {
        return _chatSessions.TryGetValue(chatSessionId, out var chatSession) ? chatSession : null;
    }
}