namespace EasyConnect.API.Services;

public interface ICurrentUserService
{
    int UserId { get; }
    string UserRole { get; }
    string UserEmail { get; }
}
