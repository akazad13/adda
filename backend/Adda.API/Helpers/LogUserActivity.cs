using System.Security.Claims;
using Adda.API.Models;
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
        ActionExecutedContext resultContext = await next();

        if (resultContext.RouteData.Values["action"]?.ToString() == "Register")
        {
            return;
        }

        int userId = int.Parse(
            resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value
        );
        IUserRepository repo =
            resultContext.HttpContext.RequestServices.GetService<IUserRepository>()
            ?? throw new InvalidOperationException(
                "IUserRepository is not registered in the service provider."
            );
        User? user = await repo.GetAsync(userId);
        if (user != null)
        {
            user.LastActive = DateTime.Now;
            _ = await repo.SaveAllAsync();
        }
    }
}
