namespace ToDoApp.Models;

public class Friend
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid FriendUserId { get; set; }
    public User FriendUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}