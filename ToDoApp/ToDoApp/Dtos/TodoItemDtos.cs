using ToDoApp.Models;

namespace ToDoApp.Dtos;

public record CreateTodoItemDto(
    string Title,
    string Description,
    PostVisibility Visibility = PostVisibility.Private
);

public record UpdateTodoItemDto(
    string Title,
    string Description,
    bool IsCompleted,
    PostVisibility Visibility
);

public record UpdateVisibilityDto(
    PostVisibility Visibility
);

public record TodoItemResponseDto(
    Guid Id,
    string Title,
    string Description,
    bool IsCompleted,
    PostVisibility Visibility,
    long Likes,
    long Dislikes,
    DateTime CreatedAt,
    Guid UserId,
    string AuthorUsername,
    bool? CurrentUserVote
);