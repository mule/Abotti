using System.IO.Abstractions;
using ChatGptBlazorCore.Models;
using ChatGptBlazorCore.QueryResults;
using Serilog;

namespace DataAccessLayer.Repositories;

public class ChatSessionFileDb : RepositoryFileDb<Guid, ChatSession>, IChatSessionRepository
{
    public ChatSessionFileDb(IFileSystem fileSystem, ILogger logger, string dbFilePath) : base(fileSystem, logger,
        dbFilePath)
    {
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