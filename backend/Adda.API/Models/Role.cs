using Microsoft.AspNetCore.Identity;

namespace Adda.API.Models;

public class Role : IdentityRole<int>
{
    public ICollection<UserRole> UserRoles { get; set; }
}
