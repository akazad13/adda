using Adda.API.Models;
using Adda.API.Security.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Adda.API.Data;

public class Seed(
    ILogger<Seed> logger,
    DataContext context,
    UserManager<User> userManager,
    RoleManager<Role> roleManager
    )
{
    private readonly ILogger<Seed> _logger = logger;
    private readonly DataContext _context = context;
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<Role> _roleManager = roleManager;

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsMySql() || _context.Database.IsSqlite())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.DatabaseInitializationError(ex);
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.DatabaseSeedingError(ex);
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        if (!await _userManager.Users.AnyAsync())
        {
            string userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            List<User>? users = userData is not null ? JsonConvert.DeserializeObject<List<User>>(userData) : [];

            // create some roles

            var roles = new List<Role>
            {
                new() { Name = RoleOption.Member },
                new() { Name = RoleOption.Admin },
                new() { Name = RoleOption.Moderator }
            };

            foreach (Role role in roles)
            {
                _ = await _roleManager.CreateAsync(role);
            }

            foreach (User user in users)
            {
                if (user.Photos.Count != 0)
                {
                    user.Photos.First().IsApproved = true;
                }
                _ = await _userManager.CreateAsync(user, "password");
                _ = await _userManager.AddToRoleAsync(user, RoleOption.Member);
            }

            // create admin user
            const string adminUsername = "Admin";
            var adminUser = new User
            {
                UserName = adminUsername,
                Email = "admin@gmail.com",
                Gender = "male",
                KnownAs = "Admin"
            };

            IdentityResult result = await _userManager.CreateAsync(adminUser, "password");
            if (result.Succeeded)
            {
                User? admin = await _userManager.FindByNameAsync(adminUsername);
                _ = await _userManager.AddToRolesAsync(admin, new[] { RoleOption.Admin, RoleOption.Moderator });
            }
        }
    }
}
