namespace ChatGptBlazorCore.Models;

public interface IUserRepository : IRepository<Guid, User>
{
    Task<(bool Ok, User? Result, string[] Errors)> GetByNameAsync(string userName);
    (bool Ok, User? Result, string[] Errors) GetByName(string userName);
}