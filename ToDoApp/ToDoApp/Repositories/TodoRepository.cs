using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Repositories;

public class TodoRepository(AppDbContext context) : ITodoRepository
{
    public async Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.TodoItems
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<TodoItem>> GetOwnTodosAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.TodoItems
            .Include(t => t.User)
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TodoItem>> GetVisibleTodosAsync(Guid currentUserId, List<Guid> friendIds, CancellationToken cancellationToken = default)
    {
        return await context.TodoItems
            .Include(t => t.User)
            .AsNoTracking()
            .Where(t => t.UserId != currentUserId &&
                (t.Visibility == PostVisibility.Public ||
                (t.Visibility == PostVisibility.FriendsOnly && friendIds.Contains(t.UserId))))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoItem> CreateAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        context.TodoItems.Add(item);
        await context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task UpdateAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        context.TodoItems.Update(item);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        context.TodoItems.Remove(item);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TodoVote?> GetVoteAsync(Guid todoId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.TodoVotes
            .FirstOrDefaultAsync(v => v.TodoItemId == todoId && v.UserId == userId, cancellationToken);
    }

    public async Task AddVoteAsync(TodoVote vote, CancellationToken cancellationToken = default)
    {
        context.TodoVotes.Add(vote);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveVoteAsync(TodoVote vote, CancellationToken cancellationToken = default)
    {
        context.TodoVotes.Remove(vote);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateVoteAsync(TodoVote vote, CancellationToken cancellationToken = default)
    {
        context.TodoVotes.Update(vote);
        await context.SaveChangesAsync(cancellationToken);
    }
}