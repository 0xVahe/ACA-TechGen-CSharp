using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Auth;

public class UserLookupService(AppDbContext context, IPasswordHasher<User> passwordHasher) : IUserLookup
{
    public async Task<UserLookupResult?> FindByCredentialsAsync(string userName, string password, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == userName, cancellationToken);

        if (user is null)
            return null;

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            return null;

        return new UserLookupResult
        {
            Id = user.Id,
            UserName = user.Username
        };
    }
}