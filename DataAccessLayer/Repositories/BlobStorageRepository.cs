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

    public bool IsInitialized { get; }


    public void Initialize()
    {
        Initialize(new Dictionary<TK, T>());
    }

    public void Initialize(IDictionary<TK, T> initialState)
    {
        if (IsInitialized)
        {
            _logger.Warning("Repository already initialized.");
            return;
        }

        try
        {
            if (_blobClient.Exists())
            {
                var dataBlob = _blobClient.Download();

                if (dataBlob.HasValue)
                {
                    var persistedData =
                        JsonSerializer.Deserialize<IDictionary<TK, T>>(dataBlob.Value.Content.ToString());
                    if (persistedData != null)
                        items = persistedData;
                    else
                        _logger.Error("{DbFile} has invalid content", _blobClient.Name);
                }
            }
            else
            {
                var data = JsonSerializer.Serialize(items);
                var dataBytes = Encoding.UTF8.GetBytes(data);
                var uploadStram = _blobClient.OpenWrite(true);
                uploadStram.Write(dataBytes);
                uploadStram.Close();


                //TODO: Handle upload failure
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to initialize repository");
            throw;
        }
    }


    public async Task InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public async Task InitializeAsync(IDictionary<TK, T> initialState)
    {
        throw new NotImplementedException();
    }
}