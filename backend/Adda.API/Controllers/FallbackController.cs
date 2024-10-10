using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adda.API.Controllers;

[AllowAnonymous]
public class FallbackController : Controller
{
    public ActionResult Index()
    {
        return PhysicalFile(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"),
            "text/html"
        );
    }
}
