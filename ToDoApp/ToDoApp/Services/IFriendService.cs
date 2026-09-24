using ToDoApp.Dtos;

namespace ToDoApp.Services;

public interface IFriendService
{
    Task<List<FriendResponseDto>> GetFriendsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<FriendAddResult> AddFriendAsync(Guid userId, Guid friendUserId, CancellationToken cancellationToken = default);
    Task<bool> RemoveFriendAsync(Guid userId, Guid friendUserId, CancellationToken cancellationToken = default);
}