namespace ToDoApp.Models;

public class TodoItem
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public PostVisibility Visibility { get; set; } = PostVisibility.Private;
    public long Likes { get; set; }
    public long Dislikes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public ICollection<TodoVote> Votes { get; set; } = new List<TodoVote>();
}