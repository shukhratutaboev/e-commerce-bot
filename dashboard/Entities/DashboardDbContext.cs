using Microsoft.EntityFrameworkCore;

namespace dashboard.Entities;
public class DashboardDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DashboardDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();
        modelBuilder.Entity<Item>()
            .HasIndex(i => i.Name)
            .IsUnique();
        modelBuilder.Entity<Category>(c => c.HasMany(c => c.Items).WithOne(i => i.Category).HasForeignKey(i => i.CategoryId));
    }
}