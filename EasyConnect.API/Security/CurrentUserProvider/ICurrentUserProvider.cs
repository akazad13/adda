namespace EasyConnect.API.Security.CurrentUserProvider;

public interface ICurrentUserProvider
{
    int UserId { get; }
    string UserRole { get; }
    string UserName { get; }
}
