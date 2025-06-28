using Microsoft.EntityFrameworkCore;
using MinTwitterApp.Models;

namespace MinTwitterApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Follow> Follows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Likes の User との関係の削除動作を Restrict に設定
        modelBuilder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasKey(f => new { f.FollowerId, f.FolloweeId });

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Followee)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FolloweeId)
            .OnDelete(DeleteBehavior.Restrict);

        // IsDeletedを見たい場合は明示的にIgnoreQueryFiltersする必要がある
        modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);
    }
}
