using Azure.Storage.Blobs;
using ChatGptBlazorCore.Models;
using ChatGptBlazorCore.QueryResults;
using Serilog;

namespace DataAccessLayer.Repositories;

public class ChatSessionBlobStorageRepository : BlobStorageRepository<Guid, ChatSession>, IChatSessionRepository
{
    public ChatSessionBlobStorageRepository(BlobClient blobClient, ILogger logger) : base(blobClient, logger)
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

    protected override (bool Ok, IDictionary<Guid, ChatSession>? Result, string[] Errors) LoadDataFromStorage()
    {
        var loadOp = base.LoadDataFromStorage();
        if (!loadOp.Ok)
            return loadOp;

        var cleanedData = loadOp.Result?.Values.Where(item => item.Entries.Any()).ToDictionary(k => k.Id, v => v);
        return (true, cleanedData, Array.Empty<string>());
    }

    protected override async Task<(bool Ok, IDictionary<Guid, ChatSession>? Result, string[] Errors)>
        LoadDataFromStorageAsync()
    {
        var loadOp = await base.LoadDataFromStorageAsync();
        if (!loadOp.Ok)
            return loadOp;

        var cleanedData = loadOp.Result?.Values.Where(item => item.Entries.Any()).ToDictionary(k => k.Id, v => v);
        return (true, cleanedData, Array.Empty<string>());
    }

    protected override (bool Ok, string[] Errors) PersistData(IDictionary<Guid, ChatSession> data)
    {
        var cleanedData = data.Values.Where(item => item.Entries.Any()).ToDictionary(k => k.Id, v => v);
        return base.PersistData(cleanedData);
    }

    protected override async Task<(bool Ok, string[] Errors)> PersistDataAsync(
        IDictionary<Guid, ChatSession> data)
    {
        var cleanedData = data.Values.Where(item => item.Entries.Any()).ToDictionary(k => k.Id, v => v);
        return await base.PersistDataAsync(cleanedData);
    }
}