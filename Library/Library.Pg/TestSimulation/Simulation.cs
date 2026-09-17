using Microsoft.EntityFrameworkCore;
using Library.Pg.Data;
using Library.Pg.Models;

namespace Library.Pg.TestSimulation;

public static class Simulation
{
    public static async Task Run()
    {
        var optionsBuilder = new DbContextOptionsBuilder<LibraryContext>();
        optionsBuilder.UseNpgsql("Host=localhost; Port=5434; Database=library; Username=postgres; Password=postgres");

        using (var db = new LibraryContext(optionsBuilder.Options))
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            Seed(db);

            RunQueries(db);

            RunUpdatesAndDeletes(db);

            await BonusSection(db);

            PostgreSqlSpecificsAsync(db);
        }
    }
    
    private static void Seed(LibraryContext db)
    {
        Console.WriteLine("=== 3.3 Observe Generated Key ===");
        var firstBook = new Book
        {
            Title = "Clean Code",
            Author = "Robert Martin",
            Year = 2008,
            Pages = 464,
            Price = 42.50m,
            IsRead = true,
            AddedAt = DateTime.UtcNow
        };

        Console.WriteLine($"BookId before SaveChanges: {firstBook.BookId}");

        var books = new List<Book>
        {
            firstBook,
            new Book { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Year = 1999, Pages = 352, Price = 38.00m, IsRead = true, AddedAt = DateTime.UtcNow },
            new Book { Title = "Designing Data-Intensive Applications", Author = "Martin Kleppmann", Year = 2017, Pages = 616, Price = 55.90m, IsRead = false, AddedAt = DateTime.UtcNow },
            new Book { Title = "Refactoring", Author = "Martin Fowler", Year = 2018, Pages = 448, Price = 47.25m, IsRead = false, AddedAt = DateTime.UtcNow },
            new Book { Title = "Code Complete", Author = "Steve McConnell", Year = 2004, Pages = 960, Price = 51.00m, IsRead = true, AddedAt = DateTime.UtcNow },
            new Book { Title = "SQL Antipatterns", Author = "Bill Karwin", Year = 2010, Pages = 328, Price = 34.75m, IsRead = false, AddedAt = DateTime.UtcNow }
        };

        db.Books.AddRange(books);
        db.SaveChanges();

        Console.WriteLine($"BookId after SaveChanges: {firstBook.BookId}\n");
    }

    private static void RunQueries(LibraryContext db)
    {
        Console.WriteLine("=== 4.1 All books ordered by year ===");
        var q41 = db.Books.OrderBy(b => b.Year).ToList();
        foreach (var b in q41)
        {
            Console.WriteLine($"{b.Year} | {b.Title} | {b.Author}");
        }

        Console.WriteLine("\n=== 4.2 Books published after 2005 ===");
        var q42 = db.Books.Where(b => b.Year > 2005).OrderBy(b => b.Title).ToList();
        foreach (var b in q42)
        {
            Console.WriteLine($"{b.Year} | {b.Title}");
        }

        Console.WriteLine("\n=== 4.3 All books by Martin Fowler ===");
        var q43 = db.Books.Where(b => b.Author == "Martin Fowler").ToList();
        foreach (var b in q43)
        {
            Console.WriteLine($"{b.Title}");
        }

        Console.WriteLine("\n=== 4.4 The longest book ===");
        var q44 = db.Books.OrderByDescending(b => b.Pages).First();
        Console.WriteLine($"Longest: {q44.Title} ({q44.Pages} pages)");

        Console.WriteLine("\n=== 4.5 Catalogue contains at least one unread book ===");
        var q45 = db.Books.Any(b => !b.IsRead);
        Console.WriteLine($"Has unread books: {q45}");

        Console.WriteLine("\n=== 4.6 Number of books already read ===");
        var q46 = db.Books.Count(b => b.IsRead);
        Console.WriteLine($"Read books count: {q46}");

        Console.WriteLine("\n=== 4.7 Titles and years projected into anonymous type ===");
        var q47 = db.Books.Select(b => new { b.Title, b.Year }).ToList();
        foreach (var item in q47)
        {
            Console.WriteLine($"{item.Title} ({item.Year})");
        }
        Console.WriteLine($"ChangeTracker entries count: {db.ChangeTracker.Entries().Count()}");

        Console.WriteLine("\n=== 4.8 Second page of results (3 per page) ===");
        var q48 = db.Books.OrderBy(b => b.Title).Skip(3).Take(3).ToList();
        foreach (var b in q48)
        {
            Console.WriteLine($"{b.Title}");
        }

        Console.WriteLine("\n=== 4.9 Look up one book twice ===");
        var firstId = db.Books.Select(b => b.BookId).First();
        var b1 = db.Books.Find(firstId);
        var b2 = db.Books.First(b => b.BookId == firstId);
        Console.WriteLine($"Same instance: {ReferenceEquals(b1, b2)}");

        Console.WriteLine("\n=== 4.10 Total price of all unread books ===");
        var q410 = db.Books.Where(b => !b.IsRead).Sum(b => b.Price);
        Console.WriteLine($"Total price: {q410}");

        Console.WriteLine("\n=== 4.11 Search for lowercase 'martin' ===");
        var search1 = db.Books.Where(b => b.Author.Contains("martin")).ToList();
        Console.WriteLine($"Contains('martin') results: {search1.Count}");
        var search2 = db.Books.Where(b => EF.Functions.ILike(b.Author, "%martin%")).ToList();
        Console.WriteLine($"ILike('%martin%') results: {search2.Count}\n");
    }

    private static void RunUpdatesAndDeletes(LibraryContext db)
    {
        Console.WriteLine("=== 5.1 Mark a book as read ===");
        var bookToUpdate = db.Books.First(b => b.Title == "SQL Antipatterns");
        Console.WriteLine($"State before modification: {db.Entry(bookToUpdate).State}");
        bookToUpdate.IsRead = true;
        Console.WriteLine($"State after modification: {db.Entry(bookToUpdate).State}");
        db.SaveChanges();

        Console.WriteLine("\n=== 5.2 Delete a book ===");
        var bookToDelete = db.Books.First(b => b.Title == "The Pragmatic Programmer");
        db.Books.Remove(bookToDelete);
        db.SaveChanges();
        Console.WriteLine($"Remaining books count: {db.Books.Count()}");

        Console.WriteLine("\n=== 5.3 Break it on purpose ===");
        var bookToBreak = db.Books.First(b => b.Title == "SQL Antipatterns");
        bookToBreak.IsRead = true;
        db.Books.Add(bookToBreak);
        try
        {
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception as expected: {ex.GetType().Name}");
        }
        db.ChangeTracker.Clear();
        Console.WriteLine();
    }

    private static async Task BonusSection(LibraryContext db)
    {
        Console.WriteLine("=== 6.1 Print SQL using ToQueryString() ===");
        var queryString = db.Books.Where(b => b.Year > 2005).OrderBy(b => b.Title).ToQueryString();
        Console.WriteLine(queryString);

        Console.WriteLine("\n=== 6.2 AsNoTracking() Comparison ===");
        db.Books.OrderBy(b => b.Year).ToList();
        Console.WriteLine($"Tracking count: {db.ChangeTracker.Entries().Count()}");
        db.ChangeTracker.Clear();
        db.Books.AsNoTracking().OrderBy(b => b.Year).ToList();
        Console.WriteLine($"NoTracking count: {db.ChangeTracker.Entries().Count()}");

        Console.WriteLine("\n=== 6.3 Dynamic Search Method ===");
        var searchResults = Search(db, 2000, "Martin", null);
        Console.WriteLine($"Search results count: {searchResults.Count}\n");
        
        await RunAsyncOperations(db);
    }

    private static async Task RunAsyncOperations(LibraryContext db)
    {
        Console.WriteLine("=== 6.4 Async Operations ===");
    
        var asyncList = await db.Books.ToListAsync();
        var asyncCount = await db.Books.CountAsync();
        var asyncAny = await db.Books.AnyAsync(b => b.IsRead);
        var asyncFirst = await db.Books.FirstAsync();
        var asyncSingle = await db.Books.SingleAsync(b => b.Title == "Clean Code");
        var asyncSum = await db.Books.Where(b => !b.IsRead).SumAsync(b => b.Price);

        Console.WriteLine($"Async Execution Complete. Loaded {asyncList.Count} books.");
        Console.WriteLine($"Total count: {asyncCount}, Any read: {asyncAny}, Single title: {asyncSingle.Title}, Sum price: {asyncSum}");
    }
    
    public static List<Book> Search(LibraryContext db, int? minYear, string? author, bool? isRead)
    {
        IQueryable<Book> query = db.Books;

        if (minYear.HasValue)
        {
            query = query.Where(b => b.Year >= minYear.Value);
        }

        if (!string.IsNullOrEmpty(author))
        {
            query = query.Where(b => b.Author.Contains(author));
        }

        if (isRead.HasValue)
        {
            query = query.Where(b => b.IsRead == isRead.Value);
        }

        return query.ToList();
    }

    private static void PostgreSqlSpecificsAsync(LibraryContext db)
    {
        Console.WriteLine("=== 7.1 DateTime UTC Requirement ===");
        try
        {
            var invalidBook = new Book
            {
                Title = "Invalid Date Book",
                Author = "Test",
                AddedAt = DateTime.Now
            };
            db.Books.Add(invalidBook);
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Caught exception: {ex.InnerException?.Message ?? ex.Message}");
            db.ChangeTracker.Clear();
        }

        Console.WriteLine("\n=== 7.3 Generated DDL ===");
        Console.WriteLine(db.Database.GenerateCreateScript());

        Console.WriteLine("=== 7.4 Server-side Grouping ===");
        var grouped = db.Books
            .GroupBy(b => b.Author)
            .Select(g => new
            {
                Author = g.Key,
                Count = g.Count(),
                Total = g.Sum(x => x.Price)
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        foreach (var item in grouped)
        {
            Console.WriteLine($"{item.Author}: {item.Count} books, Total: {item.Total}");
        }
    }
}