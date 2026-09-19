namespace ToDoApp.Dtos;

public record CreateTaskDto(string Title, string Description);

public record UpdateTaskDto(string Title, string Description, bool IsCompleted);

public record TaskResponseDto(int Id, string Title, string Description, bool IsCompleted, DateTime CreatedAt);