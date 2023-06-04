namespace ChatGptBlazorCore.Models;

public interface IChatSessionRepository
{
    ChatSession[] GetChatSessions(Guid userId);
    (Guid Id, string Topic, DateTime Created)[] GetUserChatSessionTopics(Guid userId);
    ChatSession? GetChatSession(Guid chatSessionId);
    ChatSession? AddChatSession(Guid userId, string rootPrompt);
    (bool Ok, string[] Errors) AddChatSession(ChatSession chatSession);
    (bool Ok, string[] Errors) DeleteChatSession(Guid chatSessionId);
    (bool Ok, string[] Errors) DeleteChatSessions(Guid userId);
    (bool Ok, ChatSession Result, string[] Errors) UpdateChatSession(Guid chatSessionId, string rootPrompt);
}