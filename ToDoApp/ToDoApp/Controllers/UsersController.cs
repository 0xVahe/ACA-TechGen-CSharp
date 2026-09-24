using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Auth;
using ToDoApp.Dtos;
using ToDoApp.Services;

namespace ToDoApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<UserResponseDto>> GetMe(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var user = await userService.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var success = await userService.UpdateAsync(userId, dto, cancellationToken);
        if (!success)
            return NotFound();

        return NoContent();
    }
}