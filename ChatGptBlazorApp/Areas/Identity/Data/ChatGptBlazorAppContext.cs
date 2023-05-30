using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatGptBlazorApp.Areas.Identity.Data;

public class ChatGptBlazorAppContext : IdentityDbContext<ChatGptBlazorAppUser>
{
    public ChatGptBlazorAppContext(DbContextOptions<ChatGptBlazorAppContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}