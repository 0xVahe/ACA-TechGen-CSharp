using Library.Pg.Data;
using Library.Pg.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Pg.Controllers;

[ApiController]
[Route("api/controller")]
public class LibraryController : ControllerBase
{
    private readonly LibraryContext _context;

    public LibraryController(LibraryContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetProducts()
    {
        return await _context.Books.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetProduct(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        return book;
    }
}