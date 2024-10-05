using System;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyConnect.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace EasyConnect.API.Helpers;

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
        IMemberRepository repo =
            resultContext.HttpContext.RequestServices.GetService<IMemberRepository>();
        Models.User user = await repo.GetUser(userId, true);
        user.LastActive = DateTime.Now;
        await repo.SaveAll();
    }
}
