using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace EasyConnect.API.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public int UserId => UserIdString();
    public string UserRole => UserRoleString() ?? "";
    public string UserEmail => UserEmailString() ?? "";

    private string UserEmailString()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    }

    private string UserRoleString()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
    }

    private int UserIdString()
    {
        _ = int.TryParse(
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier),
            out int userid
        );
        return userid;
    }
}
