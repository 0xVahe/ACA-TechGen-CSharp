namespace ToDoApp.Dtos;

public record RegisterDto(
    string UserName,
    string Password,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth
);

public record LoginDto(
    string UserName,
    string Password
);

public record AuthResponseDto(
    Guid UserId,
    string UserName
);