namespace ToDoApp.Models;

public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Friend> FriendsOf { get; set; } = new List<Friend>();
    public ICollection<Friend> FriendsWith { get; set; } = new List<Friend>();
    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    public ICollection<TodoVote> Votes { get; set; } = new List<TodoVote>();
}