using ChatGptBlazorCore.QueryResults;

namespace ChatGptBlazorCore.Models;

public class InMemoryChatSessionRepository : RepositoryBase<Guid, ChatSession>, IChatSessionRepository
{
    private readonly Dictionary<Guid, ChatSession> _chatSessions = new();

    public InMemoryChatSessionRepository()
    {
    }


    public InMemoryChatSessionRepository(Dictionary<Guid, ChatSession> chatSessions)
    {
        _chatSessions = chatSessions;
    }


    public async Task<(bool Ok, TopicQueryResult[] Result, string[] Errors)> GetTopicByUserIdsAsync(Guid userId)
    {
        var result = await Task.FromResult(GetTopicsByUserId(userId));
        return result;
    }

    public (bool Ok, TopicQueryResult[] Result, string[] Errors) GetTopicsByUserId(Guid userId)
    {
        var topicQueryResults = items.Values.Where(c => c.UserId == userId)
            .Select(c => new TopicQueryResult(c.Id, c.Topic, c.Created)).ToArray();

        return (true, topicQueryResults, Array.Empty<string>());
    }
}