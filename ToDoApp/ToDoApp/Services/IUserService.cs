using ToDoApp.Dtos;

namespace ToDoApp.Services;

public interface IUserService
{
    Task<UserResponseDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid userId, UpdateUserDto dto, CancellationToken cancellationToken = default);
}