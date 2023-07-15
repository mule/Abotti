using System.IO.Abstractions;
using System.Text.Json;
using ChatGptBlazorCore.Models;
using Serilog;

namespace DataAccessLayer.Repositories;

public class RepositoryFileDb<TK, T> : RepositoryBase<TK, T>, IInitializeableRepository<TK, T>
    where T : IModel<TK> where TK : notnull
{
    private readonly string _dbFilePath;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    public RepositoryFileDb(IFileSystem fileSystem, ILogger logger, string dbFilePath)
    {
        _fileSystem = fileSystem;
        _dbFilePath = dbFilePath;
        _logger = logger;
        IsInitialized = false;
    }

    public bool IsInitialized { get; private set; }


    public void Initialize()
    {
    }

    public void Initialize(IDictionary<TK, T> initialState)
    {
        throw new NotImplementedException();
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            _logger.Warning("Repository is already initialized");
            return;
        }

        if (!_fileSystem.File.Exists(_dbFilePath))
        {
            _logger?.Information("Creating new db file {DbFilePath}", _dbFilePath);
            ;
            await _fileSystem.File.WriteAllTextAsync(_dbFilePath, JsonSerializer.Serialize(items));
        }
        else
        {
            var persistedData = await LoadDbDataFromFileAsync(_fileSystem, _dbFilePath);
            if (persistedData != null)
                items = persistedData;
            else
                _logger.Warning("Failed to load data from db file {DbFilePath}", _dbFilePath);
        }

        IsInitialized = true;
    }

    public async Task InitializeAsync(IDictionary<TK, T> initialState)
    {
        items = initialState;
        await InitializeAsync();
    }

    public void Initialize(IDictionary<Guid, User> initialState)
    {
        throw new NotImplementedException();
    }


    private async Task<IDictionary<TK, T>?> LoadDbDataFromFileAsync(IFileSystem fileSystem, string dbFilePath)
    {
        using var dbDataStream = fileSystem.File.OpenText(dbFilePath);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<TK, T>>(dbDataStream.BaseStream,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
        return data;
    }
}