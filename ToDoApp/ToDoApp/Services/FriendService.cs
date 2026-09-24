using ToDoApp.Dtos;
using ToDoApp.Repositories;

namespace ToDoApp.Services;

public class FriendService(IFriendRepository friendRepository, IUserRepository userRepository) : IFriendService
{
    public async Task<List<FriendResponseDto>> GetFriendsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var friends = await friendRepository.GetFriendsAsync(userId, cancellationToken);
        return friends.Select(f => new FriendResponseDto(f.Id, f.Username, f.FirstName, f.LastName)).ToList();
    }

    public async Task<FriendAddResult> AddFriendAsync(Guid userId, Guid friendUserId, CancellationToken cancellationToken = default)
    {
        if (userId == friendUserId)
            return FriendAddResult.CannotAddSelf;

        var targetUser = await userRepository.GetByIdAsync(friendUserId, cancellationToken);
        if (targetUser is null)
            return FriendAddResult.UserNotFound;

        var areFriends = await friendRepository.AreFriendsAsync(userId, friendUserId, cancellationToken);
        if (areFriends)
            return FriendAddResult.AlreadyFriends;

        await friendRepository.AddFriendshipAsync(userId, friendUserId, cancellationToken);
        return FriendAddResult.Success;
    }

    public async Task<bool> RemoveFriendAsync(Guid userId, Guid friendUserId, CancellationToken cancellationToken = default)
    {
        return await friendRepository.RemoveFriendshipAsync(userId, friendUserId, cancellationToken);
    }
}