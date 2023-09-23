using Abotti.Core.Models;

namespace Abotti.Core.Repositories;

public interface IUserRepository : IRepository<Guid, User>
{
    Task<(bool Ok, User? Result, string[] Errors)> GetByNameAsync(string userName);
    (bool Ok, User? Result, string[] Errors) GetByName(string userName);
}