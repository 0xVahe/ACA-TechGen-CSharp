using ToDoApp.Dtos;
using ToDoApp.Models;

namespace ToDoApp.Services;

public interface ITodoService
{
    Task<List<TodoItemResponseDto>> GetMineAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<TodoItemResponseDto>> GetFeedAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(TodoItemResponseDto? Todo, TodoOperationResult Status)> GetByIdAsync(Guid todoId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<TodoItemResponseDto> CreateAsync(Guid userId, CreateTodoItemDto dto, CancellationToken cancellationToken = default);
    Task<TodoOperationResult> UpdateAsync(Guid todoId, Guid userId, UpdateTodoItemDto dto, CancellationToken cancellationToken = default);
    Task<TodoOperationResult> UpdateVisibilityAsync(Guid todoId, Guid userId, PostVisibility visibility, CancellationToken cancellationToken = default);
    Task<TodoOperationResult> VoteAsync(Guid todoId, Guid userId, bool isLike, CancellationToken cancellationToken = default);
    Task<TodoOperationResult> DeleteAsync(Guid todoId, Guid userId, CancellationToken cancellationToken = default);
}