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


    public void Initialize(bool overwrite = false)
    {
        Initialize(new Dictionary<TK, T>(), overwrite);
    }

    public void Initialize(IDictionary<TK, T> initialState, bool overwrite = false)
    {
        if (IsInitialized)
        {
            _logger.Warning("Repository is already initialized");
            return;
        }

        items = initialState;

        if (!_fileSystem.File.Exists(_dbFilePath) || overwrite)
        {
            _fileSystem.File.WriteAllText(_dbFilePath, JsonSerializer.Serialize(items));
        }
        else
        {
            var persistedData = LoadDbDataFromFile(_fileSystem, _dbFilePath);
            if (persistedData != null)
                items = persistedData;
            else
                _logger.Warning("Failed to load data from db file {DbFilePath}", _dbFilePath);
        }

        IsInitialized = true;
    }

    public async Task InitializeAsync(bool overwrite = false)
    {
        await InitializeAsync(new Dictionary<TK, T>(), overwrite);
    }

    public async Task InitializeAsync(IDictionary<TK, T> initialState, bool overwrite = false)
    {
        if (IsInitialized)
        {
            _logger.Warning("Repository is already initialized");
            return;
        }

        items = initialState;

        if (!_fileSystem.File.Exists(_dbFilePath) || overwrite)
        {
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


    public override async Task<(bool Ok, string[] Errors)> AddAsync(T item)
    {
        var result = await base.AddAsync(item);
        if (result.Ok)
        {
            var fileOperationResult = await PersistDataToFileAsync(items);
            if (!fileOperationResult.Ok)
            {
                result.Ok = false;
                result.Errors = fileOperationResult.Errors;
            }
        }

        return result;
    }

    public override async Task<(bool Ok, string[] Errors)> UpdateAsync(T item)
    {
        var result = base.Update(item);
        if (result.Ok)
        {
            var fileOperationResult = await PersistDataToFileAsync(items);
            if (!fileOperationResult.Ok)
            {
                result.Ok = false;
                result.Errors = fileOperationResult.Errors;
            }
        }

        return result;
    }

    public override (bool Ok, string[] Errors) Delete(TK key)
    {
        var deleteOpResult = base.Delete(key);
        if (!deleteOpResult.Ok)
            return deleteOpResult;

        var fileOperationResult = PersistDataToFile(items);
        return fileOperationResult;
    }

    public override async Task<(bool Ok, string[] Errors)> DeleteAsync(TK key)
    {
        var deleteOpResult = await base.DeleteAsync(key);
        if (!deleteOpResult.Ok)
            return deleteOpResult;

        var fileOperationResult = await PersistDataToFileAsync(items);
        return fileOperationResult;
    }


    protected virtual async Task<IDictionary<TK, T>?> LoadDbDataFromFileAsync(IFileSystem fileSystem, string dbFilePath)
    {
        var dbDataText = await fileSystem.File.ReadAllTextAsync(dbFilePath);
        var data = JsonSerializer.Deserialize<Dictionary<TK, T>>(dbDataText);

        return data;
    }

    protected virtual IDictionary<TK, T>? LoadDbDataFromFile(IFileSystem fileSystem, string dbFilePath)
    {
        var dbDataText = fileSystem.File.ReadAllText(dbFilePath);
        var data = JsonSerializer.Deserialize<Dictionary<TK, T>>(dbDataText);
        return data;
    }

    protected virtual (bool Ok, string[] Errors) PersistDataToFile(IDictionary<TK, T> data)
    {
        try
        {
            _fileSystem.File.WriteAllText(_dbFilePath, JsonSerializer.Serialize(data));
            return (true, Array.Empty<string>());
        }
        catch (Exception e)
        {
            var errorMessage = $"Failed to persist data to db file {_dbFilePath} because {e.Message}";
            _logger.Error(e, errorMessage, _dbFilePath);
            return (false, new[] { errorMessage });
        }
    }


    protected virtual async Task<(bool Ok, string[] Errors)> PersistDataToFileAsync(IDictionary<TK, T> data)
    {
        try
        {
            await _fileSystem.File.WriteAllTextAsync(_dbFilePath, JsonSerializer.Serialize(data));
            return (true, Array.Empty<string>());
        }
        catch (Exception e)
        {
            var errorMessage = $"Failed to persist data to db file {_dbFilePath} because {e.Message}";
            _logger.Error(e, errorMessage, _dbFilePath);
            return (false, new[] { errorMessage });
        }
    }
}