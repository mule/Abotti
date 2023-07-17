namespace ChatGptBlazorCore.Models;

public class User : IModel<Guid>
{
    public static string DefaultRole = "user";

    public User()
    {
    }

    public User(string userName) : this(Guid.NewGuid(), userName, DefaultRole)
    {
    }


    public User(Guid id, string userName, string role)
    {
        Id = id;
        UserName = userName;
        Role = role;
    }

    public string UserName { get; set; }

    public string Role { get; set; }

    public Guid Id { get; set; }
}