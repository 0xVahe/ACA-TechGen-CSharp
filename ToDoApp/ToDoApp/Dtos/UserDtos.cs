namespace ToDoApp.Dtos;

public record UpdateUserDto(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth
);

public record UserResponseDto(
    Guid Id,
    string UserName,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    DateTime CreatedAt,
    DateTime UpdatedAt
);