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
            bool isMySql = false;
            try
            {
                isMySql = _context.Database.IsMySql();
            }
            catch (InvalidOperationException)
            {
                // When running under test with InMemory provider alongside MySQL registrations,
                // IsMySql() may throw; fall back to EnsureCreated.
            }

            if (isMySql)
            {
                await _context.Database.MigrateAsync();
            }
            else
            {
                try
                {
                    await _context.Database.EnsureCreatedAsync();
                }
                catch (InvalidOperationException)
                {
                    // Tolerate mixed-provider scenarios (e.g., test environment)
                }
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
        bool hasOldData = await _context.Photos.AnyAsync(p => p.Url.Contains("randomuser.me"));
        if (hasOldData || !await _userManager.Users.AnyAsync())
        {
            _context.Photos.RemoveRange(_context.Photos);
            _context.Bookmarks.RemoveRange(_context.Bookmarks);
            _context.Messages.RemoveRange(_context.Messages);
            _context.UserRoles.RemoveRange(_context.UserRoles);
            _context.Users.RemoveRange(_context.Users);
            await _context.SaveChangesAsync();

            string path = Path.Combine(AppContext.BaseDirectory, "Data", "UserSeedData.json");
            if (!File.Exists(path))
            {
                path = "Data/UserSeedData.json";
            }
            string userData = File.Exists(path) ? await File.ReadAllTextAsync(path) : "[]";
            var users = JsonConvert.DeserializeObject<List<User>>(userData) ?? [];

            // create some roles
            if (!await _roleManager.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new() { Name = RoleOption.Member },
                    new() { Name = RoleOption.Admin },
                    new() { Name = RoleOption.Moderator }
                };

                foreach (var role in roles)
                {
                    _ = await _roleManager.CreateAsync(role);
                }
            }

            foreach (var user in users)
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

            var result = await _userManager.CreateAsync(adminUser, "password");
            if (result.Succeeded)
            {
                var admin = await _userManager.FindByNameAsync(adminUsername);
                _ = await _userManager.AddToRolesAsync(admin, new[] { RoleOption.Admin, RoleOption.Moderator });
            }
        }
    }
}
