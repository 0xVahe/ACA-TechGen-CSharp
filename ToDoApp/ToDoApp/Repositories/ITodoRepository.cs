using ToDoApp.Models;

namespace ToDoApp.Repositories;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TodoItem>> GetOwnTodosAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<TodoItem>> GetVisibleTodosAsync(Guid currentUserId, List<Guid> friendIds, CancellationToken cancellationToken = default);
    Task<TodoItem> CreateAsync(TodoItem item, CancellationToken cancellationToken = default);
    Task UpdateAsync(TodoItem item, CancellationToken cancellationToken = default);
    Task DeleteAsync(TodoItem item, CancellationToken cancellationToken = default);
    Task<TodoVote?> GetVoteAsync(Guid todoId, Guid userId, CancellationToken cancellationToken = default);
    Task AddVoteAsync(TodoVote vote, CancellationToken cancellationToken = default);
    Task RemoveVoteAsync(TodoVote vote, CancellationToken cancellationToken = default);
    Task UpdateVoteAsync(TodoVote vote, CancellationToken cancellationToken = default);
}