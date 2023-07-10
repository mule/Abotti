namespace ChatGptBlazorCore.Models;

public interface IUserRepository
{
    Task<User?> GetUserAsync(string userName);
    User? GetUser(string userName);

    Task<User?> AddUserAsync(string userName);
    User? AddUser(string userName);
}