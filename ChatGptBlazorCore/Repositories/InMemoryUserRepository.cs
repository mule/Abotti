namespace ChatGptBlazorCore.Models;

public class InMemoryUserRepository : RepositoryBase<Guid, User>, IUserRepository
{
    public InMemoryUserRepository(Guid rootUserId, string rootUserName) : this(rootUserId, rootUserName,
        new Dictionary<Guid, User>())
    {
    }

    public InMemoryUserRepository(Guid rootUserId, string rootUserName, IDictionary<Guid, User> initialState)
    {
        //remove root user from initialState if it exists
        if (initialState.ContainsKey(rootUserId))
            throw new ArgumentException("Initial state cannot contain root user id", nameof(initialState));

        //check initial state for root roles
        foreach (var user in initialState.Values)
            if (user.Role == "root")
                throw new ArgumentException("Initial state cannot contain root users", nameof(initialState));

        var rootUser = new User(rootUserName) { Id = rootUserId, Role = "root" };
        Add(rootUser);
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