using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace EasyConnect.API.Security.CurrentUserProvider;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public int UserId => GetUserId();
    public string UserRole => UserRoleString() ?? "";
    public string UserName => GetUserName();


    private string UserRoleString()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
    }

    private int GetUserId()
    {
        _ = int.TryParse(
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier),
            out int userid
        );
        return userid;
    }
    private string GetUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
    }
}
