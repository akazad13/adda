using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Adda.API.Repositories.UserRepository;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Adda.API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        ActionExecutedContext resultContext = await next();

        int userId = int.Parse(
            resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value
        );
        IUserRepository repo =
            resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
        Models.User user = await repo.GetAsync(userId, true);
        user.LastActive = DateTime.Now;
        await repo.SaveAllAsync();
    }
}
