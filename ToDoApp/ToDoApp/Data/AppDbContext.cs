using Microsoft.EntityFrameworkCore;
using ToDoApp.Models;

namespace ToDoApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Friend> Friends => Set<Friend>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<TodoVote> TodoVotes => Set<TodoVote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasMany(u => u.TodoItems)
                .WithOne(t => t.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.FriendUserId });
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                .WithMany(u => u.FriendsOf)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.FriendUser)
                .WithMany(u => u.FriendsWith)
                .HasForeignKey(e => e.FriendUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TodoItem>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Title).IsRequired().HasMaxLength(200);
            e.Property(t => t.Description).HasMaxLength(1000);
            e.Property(t => t.IsCompleted).HasDefaultValue(false);
            e.Property(p => p.Visibility).HasConversion<string>();
            e.Property(t => t.CreatedAt).HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<TodoVote>(e =>
        {
            e.HasKey(v => new { v.UserId, v.TodoItemId });

            e.HasOne(v => v.User)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(v => v.TodoItem)
                .WithMany(t => t.Votes)
                .HasForeignKey(v => v.TodoItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}