namespace ChatGptBlazorCore.Models;

public interface IChatSessionRepository
{
    bool HasChatSessions(Guid userId);
    Task<bool> HasChatSessionsAsync(Guid userId);
    ChatSession[] GetChatSessions(Guid userId);
    Task<ChatSession[]> GetChatSessionsAsync(Guid userId);

    Task<(Guid Id, string Topic, DateTime Created)[]> GetUserChatSessionTopicsAsync(Guid userId);

    (Guid Id, string Topic, DateTime Created)[] GetUserChatSessionTopics(Guid userId);

    Task<ChatSession?> GetChatSessionAsync(Guid chatSessionId, Guid userId);
    ChatSession? GetChatSession(Guid chatSessionId, Guid userId);

    Task<(bool Ok, string[] Errors)> DeleteChatSessionAsync(Guid chatSessionId);

    Task<(bool Ok, string[] Errors)> AddChatSessionAsync(ChatSession chatSession);

    (bool Ok, string[] Errors) AddChatSession(ChatSession chatSession);
    (bool Ok, string[] Errors) DeleteChatSession(Guid chatSessionId);
    (bool Ok, string[] Errors) DeleteChatSessions(Guid userId);
    (bool Ok, ChatSession Result, string[] Errors) UpdateChatSession(ChatSession chatSession);
    Task<(bool Ok, ChatSession Result, string[] Errors)> UpdateChatSessionAsync(ChatSession chatSession);
}