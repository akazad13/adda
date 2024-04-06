using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyConnect.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EasyConnect.API.Data
{
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
                _logger.LogError(ex, "An error occurred while initialising the database.");
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
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            if (!await _userManager.Users.AnyAsync())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                // create some roles

                var roles = new List<Role>
                {
                    new() { Name = "Member" },
                    new() { Name = "Admin" },
                    new() { Name = "Moderator" }
                };

                foreach (var role in roles)
                {
                    await _roleManager.CreateAsync(role);
                }

                foreach (var user in users)
                {
                    if (user.Photos.Any())
                    {
                        user.Photos.First().IsApproved = true;
                    }
                    await _userManager.CreateAsync(user, "password");
                    await _userManager.AddToRoleAsync(user, "Member");
                }

                // create admin user
                var adminUser = new User
                {
                    UserName = "Admin",
                    Email = "admin@gmail.com",
                    Gender = "male",
                    KnownAs = "Admin"
                };

                var result = await _userManager.CreateAsync(adminUser, "password");
                if (result.Succeeded)
                {
                    var admin = await _userManager.FindByNameAsync("Admin");
                    await _userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
                }
            }
        }
    }
}
