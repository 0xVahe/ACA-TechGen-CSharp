using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Auth;
using ToDoApp.Dtos;
using ToDoApp.Services;

namespace ToDoApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class FriendsController(IFriendService friendService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<FriendResponseDto>>> GetFriends(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var friends = await friendService.GetFriendsAsync(userId, cancellationToken);
        return Ok(friends);
    }

    [HttpPost]
    public async Task<IActionResult> AddFriend(AddFriendDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await friendService.AddFriendAsync(userId, dto.FriendUserId, cancellationToken);

        return result switch
        {
            FriendAddResult.Success => Ok(),
            FriendAddResult.CannotAddSelf => BadRequest(new { message = "Cannot add yourself as a friend." }),
            FriendAddResult.UserNotFound => NotFound(new { message = "User not found." }),
            FriendAddResult.AlreadyFriends => Conflict(new { message = "Already friends." }),
            _ => BadRequest()
        };
    }

    [HttpDelete("{friendUserId:guid}")]
    public async Task<IActionResult> RemoveFriend(Guid friendUserId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var success = await friendService.RemoveFriendAsync(userId, friendUserId, cancellationToken);
        if (!success)
            return NotFound(new { message = "Friendship record not found." });

        return NoContent();
    }
}