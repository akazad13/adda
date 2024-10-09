using System.Security.Claims;
using Adda.API.Repositories.UserRepository;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Adda.API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        ArgumentNullException.ThrowIfNull(next);

        var resultContext = await next();

        if (resultContext.RouteData.Values["action"]?.ToString() == "Register")
        {
            return;
        }

        int userId = int.Parse(
            resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value
        );
        var repo =
            resultContext.HttpContext.RequestServices.GetService<IUserRepository>()
            ?? throw new InvalidOperationException(
                "IUserRepository is not registered in the service provider."
            );
        var user = await repo.GetAsync(userId);
        if (user != null)
        {
            user.LastActive = DateTime.Now;
            _ = await repo.SaveAllAsync();
        }
    }
}
