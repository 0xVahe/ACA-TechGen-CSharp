using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Auth;
using ToDoApp.Dtos;
using ToDoApp.Services;

namespace ToDoApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TodoController(ITodoService todoService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TodoItemResponseDto>>> GetMine(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var todos = await todoService.GetMineAsync(userId, cancellationToken);
        return Ok(todos);
    }

    [HttpGet("public")]
    public async Task<ActionResult<List<TodoItemResponseDto>>> GetFeed(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var todos = await todoService.GetFeedAsync(userId, cancellationToken);
        return Ok(todos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoItemResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var (todo, status) = await todoService.GetByIdAsync(id, userId, cancellationToken);

        return status switch
        {
            TodoOperationResult.Success => Ok(todo),
            TodoOperationResult.NotFound => NotFound(),
            TodoOperationResult.Forbidden => StatusCode(StatusCodes.Status403Forbidden),
            _ => BadRequest()
        };
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemResponseDto>> Create(CreateTodoItemDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var created = await todoService.CreateAsync(userId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTodoItemDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var status = await todoService.UpdateAsync(id, userId, dto, cancellationToken);

        return status switch
        {
            TodoOperationResult.Success => NoContent(),
            TodoOperationResult.NotFound => NotFound(),
            TodoOperationResult.Forbidden => StatusCode(StatusCodes.Status403Forbidden),
            _ => BadRequest()
        };
    }

    [HttpPatch("{id:guid}/visible")]
    public async Task<IActionResult> UpdateVisibility(Guid id, UpdateVisibilityDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var status = await todoService.UpdateVisibilityAsync(id, userId, dto.Visibility, cancellationToken);

        return status switch
        {
            TodoOperationResult.Success => Ok(),
            TodoOperationResult.NotFound => NotFound(),
            TodoOperationResult.Forbidden => StatusCode(StatusCodes.Status403Forbidden),
            _ => BadRequest()
        };
    }

    [HttpPost("{id:guid}/like")]
    public async Task<IActionResult> Like(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var status = await todoService.VoteAsync(id, userId, true, cancellationToken);

        return status switch
        {
            TodoOperationResult.Success => Ok(),
            TodoOperationResult.NotFound => NotFound(),
            TodoOperationResult.Forbidden => StatusCode(StatusCodes.Status403Forbidden),
            TodoOperationResult.Invalid => BadRequest(new { message = "Cannot vote on your own post." }),
            _ => BadRequest()
        };
    }

    [HttpPost("{id:guid}/dislike")]
    public async Task<IActionResult> Dislike(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var status = await todoService.VoteAsync(id, userId, false, cancellationToken);

        return status switch
        {
            TodoOperationResult.Success => Ok(),
            TodoOperationResult.NotFound => NotFound(),
            TodoOperationResult.Forbidden => StatusCode(StatusCodes.Status403Forbidden),
            TodoOperationResult.Invalid => BadRequest(new { message = "Cannot vote on your own post." }),
            _ => BadRequest()
        };
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var status = await todoService.DeleteAsync(id, userId, cancellationToken);

        return status switch
        {
            TodoOperationResult.Success => NoContent(),
            TodoOperationResult.NotFound => NotFound(),
            TodoOperationResult.Forbidden => StatusCode(StatusCodes.Status403Forbidden),
            _ => BadRequest()
        };
    }
}