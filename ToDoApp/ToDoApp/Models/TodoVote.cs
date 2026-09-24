namespace ToDoApp.Models;

public class TodoVote
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid TodoItemId { get; set; }
    public TodoItem TodoItem { get; set; } = null!;

    public bool IsLike { get; set; }
}