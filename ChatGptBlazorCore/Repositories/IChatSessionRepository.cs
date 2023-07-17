using ChatGptBlazorCore.QueryResults;

namespace ChatGptBlazorCore.Models;

public interface IChatSessionRepository : IRepository<Guid, ChatSession>
{
    Task<(bool Ok, TopicQueryResult[] Result, string[] Errors)> GetTopicByUserIdsAsync(Guid userId);

    (bool Ok, TopicQueryResult[] Result, string[] Errors) GetTopicsByUserId(Guid userId);
}