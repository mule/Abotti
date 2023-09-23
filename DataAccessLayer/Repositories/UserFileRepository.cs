using System.IO.Abstractions;
using Abotti.Core.Models;
using Abotti.Core.Repositories;
using Serilog;

namespace Abotti.DataAccessLayer.Repositories;

public class UserFileRepository : FileSystemRepository<Guid, User>, IUserRepository
{
    public UserFileRepository(IFileSystem fileSystem, ILogger logger, string dbFilePath) : base(fileSystem, logger,
        dbFilePath)
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