namespace ToDoApp.Dtos;

public record AddFriendDto(
    Guid FriendUserId
);

public record FriendResponseDto(
    Guid Id,
    string UserName,
    string FirstName,
    string LastName
);