using System.IO.Abstractions;
using ChatGptBlazorCore.Models;
using ChatGptBlazorCore.QueryResults;
using Serilog;

namespace DataAccessLayer.Repositories;

public class ChatSessionFileRepository : FileSystemRepository<Guid, ChatSession>, IChatSessionRepository
{
    public ChatSessionFileRepository(IFileSystem fileSystem, ILogger logger, string dbFilePath) : base(fileSystem,
        logger,
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

    protected override async Task<IDictionary<Guid, ChatSession>?> LoadDbDataFromFileAsync(IFileSystem fileSystem,
        string dbFilePath)
    {
        var data = await base.LoadDbDataFromFileAsync(fileSystem, dbFilePath);

        var cleanedData = data?.Values.Where(item => item.Entries.Any()).ToDictionary(k => k.Id, v => v);
        return cleanedData;
    }

    protected override IDictionary<Guid, ChatSession>? LoadDbDataFromFile(IFileSystem fileSystem, string dbFilePath)
    {
        var data = base.LoadDbDataFromFile(fileSystem, dbFilePath);

        var cleanedData = data?.Values.Where(item => item.Entries.Any()).ToDictionary(k => k.Id, v => v);
        return cleanedData;
    }

    protected override async Task<(bool Ok, string[] Errors)> PersistDataToFileAsync(
        IDictionary<Guid, ChatSession> data)
    {
        var cleanedData = data.Values.Where(item => item.Entries.Any()).ToDictionary(k => k.Id, v => v);
        return await base.PersistDataToFileAsync(cleanedData);
    }
}