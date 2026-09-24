using ToDoApp.Dtos;
using ToDoApp.Repositories;

namespace ToDoApp.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserResponseDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return null;

        return new UserResponseDto(
            user.Id,
            user.Username,
            user.FirstName,
            user.LastName,
            user.DateOfBirth,
            user.CreatedAt,
            user.UpdatedAt
        );
    }

    public async Task<bool> UpdateAsync(Guid userId, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return false;

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.DateOfBirth = dto.DateOfBirth;

        await userRepository.UpdateAsync(user, cancellationToken);
        return true;
    }
}