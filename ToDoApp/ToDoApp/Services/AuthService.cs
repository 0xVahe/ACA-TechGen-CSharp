using Microsoft.AspNetCore.Identity;
using ToDoApp.Dtos;
using ToDoApp.Models;
using ToDoApp.Repositories;

namespace ToDoApp.Services;

public class AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher) : IAuthService
{
    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        if (await userRepository.ExistsByUsernameAsync(dto.UserName, cancellationToken))
            return null;

        var user = new User
        {
            Username = dto.UserName,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth
        };

        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
        var created = await userRepository.CreateAsync(user, cancellationToken);

        return new AuthResponseDto(created.Id, created.Username);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByUsernameAsync(dto.UserName, cancellationToken);
        if (user is null)
            return null;

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return null;

        return new AuthResponseDto(user.Id, user.Username);
    }
}