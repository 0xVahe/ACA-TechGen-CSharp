using ToDoApp.Models;

namespace ToDoApp.Repositories;

public interface IFriendRepository
{
    Task<bool> AreFriendsAsync(Guid userId, Guid friendUserId, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetFriendIdsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<User>> GetFriendsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> AddFriendshipAsync(Guid userId, Guid friendUserId, CancellationToken cancellationToken = default);
    Task<bool> RemoveFriendshipAsync(Guid userId, Guid friendUserId, CancellationToken cancellationToken = default);
}