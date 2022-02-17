using Microsoft.EntityFrameworkCore;

namespace bot.Entities;
public class BotDbContext : DbContext
{
    public DbSet<User>? Users { get; set; }
    public BotDbContext(DbContextOptions<BotDbContext> options)
        :base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        base.OnConfiguring(options);
    }
}