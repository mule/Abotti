using Abotti.Core.Models;
using Abotti.Core.Repositories;
using Azure.Storage.Blobs;
using Serilog;

namespace Abotti.DataAccessLayer.Repositories;

public class UserBlobStorageRepo : BlobStorageRepository<Guid, User>, IUserRepository
{
    public UserBlobStorageRepo(BlobClient blobClient, ILogger logger) : base(blobClient, logger)
    {
    }

    public async Task<(bool Ok, User? Result, string[] Errors)> GetByNameAsync(string userName)
    {
        var result = await Task.FromResult(GetByName(userName));
        return result;
    }

    public (bool Ok, User? Result, string[] Errors) GetByName(string userName)
    {
        var user = items.Values.FirstOrDefault(u => u.UserName == userName);
        return (user != null, user, user != null ? Array.Empty<string>() : new[] { "User not found" });
    }
}