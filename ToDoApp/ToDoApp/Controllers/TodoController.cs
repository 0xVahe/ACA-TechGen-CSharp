using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Dtos;
using ToDoApp.Models;

namespace ToDoApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TaskResponseDto>>> GetAll()
    {
        var tasks = await context.Tasks
            .AsNoTracking()
            .Select(t => new TaskResponseDto(t.Id, t.Title, t.Description, t.IsCompleted, t.CreatedAt))
            .ToListAsync();

        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponseDto>> GetById(int id)
    {
        var task = await context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
            return NotFound();

        return Ok(new TaskResponseDto(task.Id, task.Title, task.Description, task.IsCompleted, task.CreatedAt));
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> Create(CreateTaskDto dto)
    {
        var task = new TaskEntity
        {
            Title = dto.Title,
            Description = dto.Description
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var response = new TaskResponseDto(task.Id, task.Title, task.Description, task.IsCompleted, task.CreatedAt);
        
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTaskDto dto)
    {
        var task = await context.Tasks.FindAsync(id);

        if (task is null)
            return NotFound();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;

        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await context.Tasks.FindAsync(id);

        if (task is null)
            return NotFound();

        context.Tasks.Remove(task);
        await context.SaveChangesAsync();

        return NoContent();
    }
}