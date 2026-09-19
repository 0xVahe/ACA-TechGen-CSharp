using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ToDoApp.Models;

namespace ToDoApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseNpgsql("Host=localhost;Port=5435;Database=todo;Username=postgres;Password=postgres");
        }

        options.LogTo(
            Console.WriteLine,
            [ DbLoggerCategory.Database.Command.Name ],
            LogLevel.Information,
            DbContextLoggerOptions.SingleLine
        );
        options.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Title).IsRequired().HasMaxLength(200);
            e.Property(t => t.Description).HasMaxLength(1000);
            e.Property(t => t.IsCompleted).HasDefaultValue(false);
            e.Property(t => t.CreatedAt).HasDefaultValueSql("NOW()");
        });
    }
}