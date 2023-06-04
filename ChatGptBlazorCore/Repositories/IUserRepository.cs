namespace ChatGptBlazorCore.Models;

public interface IUserRepository
{
    User? GetUser(string userName);
    User? AddUser(string userName);
}