using Library.Pg.Data;
using Library.Pg.HealthChecks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseNpgsql("Host=localhost; Port=5434; Database=library; Username=postgres; Password=postgres"));

builder.Services.AddHttpClient("LibraryApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7001/"); 
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHealthChecks()
    .AddCheck<LibraryRoutesHealthCheck>("library_http_routes_check");

builder.Services.AddControllers();

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();