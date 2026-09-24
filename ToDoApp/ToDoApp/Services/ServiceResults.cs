namespace ToDoApp.Services;

public enum FriendAddResult
{
    Success,
    CannotAddSelf,
    UserNotFound,
    AlreadyFriends
}

public enum TodoOperationResult
{
    Success,
    NotFound,
    Forbidden,
    Invalid
}