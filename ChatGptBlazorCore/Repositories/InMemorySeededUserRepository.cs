namespace ChatGptBlazorCore.Models;

public class InMemorySeededUserRepository : IUserRepository
{
    private readonly Dictionary<string, User?> _users = new();

    public InMemorySeededUserRepository()
    {
    }

    public InMemorySeededUserRepository(Guid adminUserId, string adminUserName)
    {
        _users.Add(adminUserName, new User(adminUserName) { Id = adminUserId });
    }

    public User? GetUser(string userName)
    {
        if (userName == null) throw new ArgumentNullException(nameof(userName));
        return _users.TryGetValue(userName, out var user) ? user : null;
    }

    public User? AddUser(string userName)
    {
        if (userName == null) throw new ArgumentNullException(nameof(userName));
        User? user;

        if (_users.TryGetValue(userName, out user))
            return user;

        user = new User(userName);
        _users.Add(userName, user);
        return user;
    }
}