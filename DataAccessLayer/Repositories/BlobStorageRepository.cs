using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using ChatGptBlazorCore.Models;
using Serilog;

namespace DataAccessLayer.Repositories;

public class BlobStorageRepository<TK, T> : RepositoryBase<TK, T>, IInitializeableRepository<TK, T>
    where T : IModel<TK> where TK : notnull
{
    private readonly BlobClient _blobClient;
    private readonly ILogger _logger;

    public BlobStorageRepository(BlobClient blobClient, ILogger logger)
    {
        _blobClient = blobClient;
        _logger = logger;
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
            _logger.Warning("Repository already initialized.");
            return;
        }

        items = initialState;

        try
        {
            if (_blobClient.Exists() && !overwrite)
            {
                var loadOp = LoadDataFromStorage();

                if (loadOp.Ok && loadOp.Result != null)
                    items = loadOp.Result;
                else
                    _logger.Error("Failed to load data from blob {BlobName}", _blobClient.Name);
            }
            else
            {
                var persistOp = PersistData(items);
                if (!persistOp.Ok)
                    _logger.Error("Failed to persist data to blob {BlobName}", _blobClient.Name);
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to initialize repository");
            throw;
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
            _logger.Warning("Repository already initialized.");
            return;
        }

        items = initialState;

        try
        {
            if (await _blobClient.ExistsAsync() && !overwrite)
            {
                var loadOp = await LoadDataFromStorageAsync();
                if (loadOp.Ok && loadOp.Result != null)
                    items = loadOp.Result;
                else
                    _logger.Error("Failed to load data from blob {BlobName}", _blobClient.Name);
            }
            else
            {
                var persistOp = await PersistDataAsync(items);
                if (!persistOp.Ok)
                    _logger.Error("Failed to persist data to blob {BlobName}", _blobClient.Name);
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to initialize repository");
            throw;
        }

        IsInitialized = true;
    }

    public override (bool Ok, string[] Errors) Add(T item)
    {
        var addOpResult = base.Add(item);
        if (!addOpResult.Ok)
        {
            _logger.Error("Failed to add item to {DbFile}", _blobClient.Name);
            return addOpResult;
        }

        var persistOpResult = PersistData(items);
        if (!persistOpResult.Ok) _logger.Error("Failed to persist data to {DbFile}", _blobClient.Name);

        return persistOpResult;
    }

    public override async Task<(bool Ok, string[] Errors)> AddAsync(T item)
    {
        var addOpResult = await base.AddAsync(item);

        if (!addOpResult.Ok)
        {
            _logger.Error("Failed to add item to {DbFile}", _blobClient.Name);
            return addOpResult;
        }

        var persistResult = await PersistDataAsync(items);
        if (!persistResult.Ok)
        {
            _logger.Error("Failed to persist data to {DbFile}", _blobClient.Name);
            return persistResult;
        }

        return (true, Array.Empty<string>());
    }

    public override (bool Ok, string[] Errors) Update(T item)
    {
        var updateOpResult = base.Update(item);
        if (!updateOpResult.Ok)
        {
            _logger.Error("Failed to update item in {DbFile}", _blobClient.Name);
            return updateOpResult;
        }

        var persistOpResult = PersistData(items);
        if (!persistOpResult.Ok) _logger.Error("Failed to persist data to {DbFile}", _blobClient.Name);

        return persistOpResult;
    }

    public override async Task<(bool Ok, string[] Errors)> UpdateAsync(T item)
    {
        var updateOpResult = await base.UpdateAsync(item);
        if (!updateOpResult.Ok)
        {
            _logger.Error("Failed to update item in {DbFile}", _blobClient.Name);
            return updateOpResult;
        }

        var persistOpResult = await PersistDataAsync(items);
        if (!persistOpResult.Ok) _logger.Error("Failed to persist data to {DbFile}", _blobClient.Name);

        return persistOpResult;
    }

    public override (bool Ok, string[] Errors) Delete(TK key)
    {
        var deleteOpResult = base.Delete(key);

        if (!deleteOpResult.Ok)
        {
            _logger.Error("Failed to delete item from {DbFile}", _blobClient.Name);
            return deleteOpResult;
        }

        var persistOpResult = PersistData(items);
        return persistOpResult;
    }

    public override async Task<(bool Ok, string[] Errors)> DeleteAsync(TK key)
    {
        var deleteOpResult = await base.DeleteAsync(key);
        if (!deleteOpResult.Ok)
        {
            _logger.Error("Failed to delete item from {DbFile}", _blobClient.Name);
            return deleteOpResult;
        }

        var persistOpResult = await PersistDataAsync(items);
        return persistOpResult;
    }

    protected virtual (bool Ok, string[] Errors) PersistData(IDictionary<TK, T> data)
    {
        try
        {
            var writeStream = _blobClient.OpenWrite(true);
            var serializedData = JsonSerializer.Serialize(data);
            var dataBytes = Encoding.UTF8.GetBytes(serializedData);
            writeStream.Write(dataBytes);
            writeStream.Close();
            return (true, Array.Empty<string>());
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to persist data to {DbFile}", _blobClient.Name);
            return (false, new[] { e.Message });
        }
    }

    protected virtual async Task<(bool Ok, string[] Errors)> PersistDataAsync(IDictionary<TK, T> data)
    {
        try
        {
            var writeStream = await _blobClient.OpenWriteAsync(true);
            var serializedData = JsonSerializer.Serialize(data);
            var dataBytes = Encoding.UTF8.GetBytes(serializedData);
            await writeStream.WriteAsync(dataBytes);
            writeStream.Close();
            return (true, Array.Empty<string>());
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to persist data to {DbFile}", _blobClient.Name);
            return (false, new[] { e.Message });
        }
    }

    protected virtual (bool Ok, IDictionary<TK, T>? Result, string[] Errors) LoadDataFromStorage()
    {
        try
        {
            var dataBlob = _blobClient.DownloadContent();
            if (!dataBlob.HasValue)
            {
                var error = $"{_blobClient.Name} does not exist";
                _logger.Error(error);
                return (false, null, new[] { error });
            }

            var persistedData =
                JsonSerializer.Deserialize<IDictionary<TK, T>>(dataBlob.Value.Content.ToString());
            if (persistedData == null)
            {
                var error = $"{_blobClient.Name} has invalid content";
                _logger.Error(error);
                return (false, null, new[] { error });
            }

            return (true, persistedData, Array.Empty<string>());
        }
        catch (Exception e)
        {
            var error = $"Failed to load data from {_blobClient.Name}";
            _logger.Error(e, error);
            return (false, null, new[] { error });
        }
    }

    protected virtual async Task<(bool Ok, IDictionary<TK, T>? Result, string[] Errors)> LoadDataFromStorageAsync()
    {
        try
        {
            var dataBlob = await _blobClient.DownloadContentAsync();

            if (!dataBlob.HasValue || dataBlob.Value.Content == null)
            {
                var error = $"{_blobClient.Name} does not exist";
                _logger.Error(error);
                return (false, null, new[] { error });
            }


            var persistedData =
                JsonSerializer.Deserialize<IDictionary<TK, T>>(dataBlob.Value.Content);
            if (persistedData == null)
            {
                var error = $"{_blobClient.Name} has invalid content";
                _logger.Error(error);
                return (false, null, new[] { error });
            }

            return (true, persistedData, Array.Empty<string>());
        }
        catch (Exception e)
        {
            var error = $"Failed to load data from {_blobClient.Name}";
            _logger.Error(e, error);
            return (false, null, new[] { error });
        }
    }
}