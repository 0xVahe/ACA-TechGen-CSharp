using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Library.Pg.Models;

namespace Library.Pg.Data;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseNpgsql("Host=localhost; Port=5434; Database=library; Username=postgres; Password=postgres");
        }

        options.LogTo(
            Console.WriteLine,
            new[] { DbLoggerCategory.Database.Command.Name },
            LogLevel.Information,
            DbContextLoggerOptions.SingleLine
        );
        options.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(e =>
        {
            e.Property(b => b.Title).IsRequired().HasMaxLength(200);
            e.Property(b => b.Author).IsRequired().HasMaxLength(120);
            e.Property(b => b.Price).HasPrecision(10, 2);
            e.HasIndex(b => b.Author).IsUnique(false);
        });
    }
}