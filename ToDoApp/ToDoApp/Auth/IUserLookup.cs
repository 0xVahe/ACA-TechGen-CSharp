namespace ToDoApp.Auth;

public class UserLookupResult
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public interface IUserLookup
{
    Task<UserLookupResult?> FindByCredentialsAsync(string userName, string password, CancellationToken cancellationToken);
}