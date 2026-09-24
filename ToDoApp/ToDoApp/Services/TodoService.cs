using ToDoApp.Dtos;
using ToDoApp.Models;
using ToDoApp.Repositories;

namespace ToDoApp.Services;

public class TodoService(ITodoRepository todoRepository, IFriendRepository friendRepository) : ITodoService
{
    public async Task<List<TodoItemResponseDto>> GetMineAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var todos = await todoRepository.GetOwnTodosAsync(userId, cancellationToken);
        return await MapToResponseListAsync(todos, userId, cancellationToken);
    }

    public async Task<List<TodoItemResponseDto>> GetFeedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var friendIds = await friendRepository.GetFriendIdsAsync(userId, cancellationToken);
        var todos = await todoRepository.GetVisibleTodosAsync(userId, friendIds, cancellationToken);
        return await MapToResponseListAsync(todos, userId, cancellationToken);
    }

    public async Task<(TodoItemResponseDto? Todo, TodoOperationResult Status)> GetByIdAsync(Guid todoId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var todo = await todoRepository.GetByIdAsync(todoId, cancellationToken);
        if (todo is null)
            return (null, TodoOperationResult.NotFound);

        if (todo.UserId != currentUserId)
        {
            if (todo.Visibility == PostVisibility.Private)
                return (null, TodoOperationResult.Forbidden);

            if (todo.Visibility == PostVisibility.FriendsOnly)
            {
                var isFriend = await friendRepository.AreFriendsAsync(currentUserId, todo.UserId, cancellationToken);
                if (!isFriend)
                    return (null, TodoOperationResult.Forbidden);
            }
        }

        var vote = await todoRepository.GetVoteAsync(todo.Id, currentUserId, cancellationToken);
        var dto = MapToDto(todo, vote?.IsLike);
        return (dto, TodoOperationResult.Success);
    }

    public async Task<TodoItemResponseDto> CreateAsync(Guid userId, CreateTodoItemDto dto, CancellationToken cancellationToken = default)
    {
        var todo = new TodoItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Visibility = dto.Visibility,
            UserId = userId
        };

        var created = await todoRepository.CreateAsync(todo, cancellationToken);
        return MapToDto(created, null);
    }

    public async Task<TodoOperationResult> UpdateAsync(Guid todoId, Guid userId, UpdateTodoItemDto dto, CancellationToken cancellationToken = default)
    {
        var todo = await todoRepository.GetByIdAsync(todoId, cancellationToken);
        if (todo is null)
            return TodoOperationResult.NotFound;

        if (todo.UserId != userId)
            return TodoOperationResult.Forbidden;

        todo.Title = dto.Title;
        todo.Description = dto.Description;
        todo.IsCompleted = dto.IsCompleted;
        todo.Visibility = dto.Visibility;

        await todoRepository.UpdateAsync(todo, cancellationToken);
        return TodoOperationResult.Success;
    }

    public async Task<TodoOperationResult> UpdateVisibilityAsync(Guid todoId, Guid userId, PostVisibility visibility, CancellationToken cancellationToken = default)
    {
        var todo = await todoRepository.GetByIdAsync(todoId, cancellationToken);
        if (todo is null)
            return TodoOperationResult.NotFound;

        if (todo.UserId != userId)
            return TodoOperationResult.Forbidden;

        todo.Visibility = visibility;
        await todoRepository.UpdateAsync(todo, cancellationToken);
        return TodoOperationResult.Success;
    }

    public async Task<TodoOperationResult> VoteAsync(Guid todoId, Guid userId, bool isLike, CancellationToken cancellationToken = default)
    {
        var todo = await todoRepository.GetByIdAsync(todoId, cancellationToken);
        if (todo is null)
            return TodoOperationResult.NotFound;

        if (todo.UserId == userId)
            return TodoOperationResult.Invalid;

        if (todo.Visibility == PostVisibility.Private)
            return TodoOperationResult.Forbidden;

        if (todo.Visibility == PostVisibility.FriendsOnly)
        {
            var isFriend = await friendRepository.AreFriendsAsync(userId, todo.UserId, cancellationToken);
            if (!isFriend)
                return TodoOperationResult.Forbidden;
        }

        var existingVote = await todoRepository.GetVoteAsync(todoId, userId, cancellationToken);
        if (existingVote is null)
        {
            await todoRepository.AddVoteAsync(new TodoVote { UserId = userId, TodoItemId = todoId, IsLike = isLike }, cancellationToken);
            if (isLike) todo.Likes++; else todo.Dislikes++;
        }
        else if (existingVote.IsLike == isLike)
        {
            await todoRepository.RemoveVoteAsync(existingVote, cancellationToken);
            if (isLike) todo.Likes--; else todo.Dislikes--;
        }
        else
        {
            existingVote.IsLike = isLike;
            await todoRepository.UpdateVoteAsync(existingVote, cancellationToken);
            if (isLike)
            {
                todo.Likes++;
                todo.Dislikes--;
            }
            else
            {
                todo.Likes--;
                todo.Dislikes++;
            }
        }

        await todoRepository.UpdateAsync(todo, cancellationToken);
        return TodoOperationResult.Success;
    }

    public async Task<TodoOperationResult> DeleteAsync(Guid todoId, Guid userId, CancellationToken cancellationToken = default)
    {
        var todo = await todoRepository.GetByIdAsync(todoId, cancellationToken);
        if (todo is null)
            return TodoOperationResult.NotFound;

        if (todo.UserId != userId)
            return TodoOperationResult.Forbidden;

        await todoRepository.DeleteAsync(todo, cancellationToken);
        return TodoOperationResult.Success;
    }

    private async Task<List<TodoItemResponseDto>> MapToResponseListAsync(List<TodoItem> todos, Guid currentUserId, CancellationToken cancellationToken)
    {
        var result = new List<TodoItemResponseDto>();
        foreach (var t in todos)
        {
            var vote = await todoRepository.GetVoteAsync(t.Id, currentUserId, cancellationToken);
            result.Add(MapToDto(t, vote?.IsLike));
        }
        return result;
    }

    private static TodoItemResponseDto MapToDto(TodoItem todo, bool? userVote)
    {
        return new TodoItemResponseDto(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.Visibility,
            todo.Likes,
            todo.Dislikes,
            todo.CreatedAt,
            todo.UserId,
            todo.User?.Username ?? string.Empty,
            userVote
        );
    }
}