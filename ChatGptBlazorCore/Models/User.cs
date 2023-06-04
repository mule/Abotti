namespace ChatGptBlazorCore.Models;

public class User
{
    public User(string userName)
    {
        Id = Guid.NewGuid();
        UserName = userName;
    }

    public Guid Id { get; set; }
    public string UserName { get; set; }
}