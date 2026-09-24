using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Repositories;

public class FriendRepository(AppDbContext context) : IFriendRepository
{
    public async Task<bool> AreFriendsAsync(Guid userId, Guid friendUserId, CancellationToken cancellationToken = default)
    {
        return await context.Friends.AnyAsync(f => f.UserId == userId && f.FriendUserId == friendUserId, cancellationToken);
    }

    public async Task<List<Guid>> GetFriendIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Friends
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .Select(f => f.FriendUserId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetFriendsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Friends
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .Select(f => f.FriendUser)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AddFriendshipAsync(Guid userId, Guid friendUserId, CancellationToken cancellationToken = default)
    {
        var f1 = new Friend { UserId = userId, FriendUserId = friendUserId };
        var f2 = new Friend { UserId = friendUserId, FriendUserId = userId };

        context.Friends.AddRange(f1, f2);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveFriendshipAsync(Guid userId, Guid friendUserId, CancellationToken cancellationToken = default)
    {
        var records = await context.Friends
            .Where(f => (f.UserId == userId && f.FriendUserId == friendUserId) ||
                        (f.UserId == friendUserId && f.FriendUserId == userId))
            .ToListAsync(cancellationToken);

        if (records.Count == 0)
            return false;

        context.Friends.RemoveRange(records);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}