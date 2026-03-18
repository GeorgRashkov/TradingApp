using Microsoft.AspNetCore.Identity;
using TradingApp.Data.Models;
using TradingApp.GCommon;

namespace TradingApp.Data.Seed
{
    public class UserSeeder
    {
        private readonly UserManager<User> _userManager;
        RoleManager<IdentityRole> _roleManager;

        public UserSeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            string userRole = _roleManager.Roles.FirstOrDefault(r => r.Name == ApplicationRoles.User)?.Name ?? throw new InvalidOperationException("The role `User` was not found in the DB!");
            string adminRole = _roleManager.Roles.FirstOrDefault(r => r.Name == ApplicationRoles.Admin)?.Name ?? throw new InvalidOperationException("The role `Admin` was not found in the DB!");
            string moderatorRole = _roleManager.Roles.FirstOrDefault(r => r.Name == ApplicationRoles.Moderator)?.Name ?? throw new InvalidOperationException("The role `Moderator` was not found in the DB!");

            var users = new[]
            {
                new { UserName = "alice",   Email = "alice@test.com", Role = userRole  },
                new { UserName = "bob",     Email = "bob@test.com", Role = userRole },
                new { UserName = "charlie", Email = "charlie@test.com", Role = userRole },
                new { UserName = "denis",   Email = "denis@test.com", Role = userRole },
                new { UserName = "edward",  Email = "edward@test.com", Role = userRole },
                new { UserName = "frank",   Email = "frank@test.com", Role = userRole },
                new { UserName = "grace",   Email = "grace@test.com", Role = userRole },

                new { UserName = "admin1",   Email = "admin1@test.com", Role = adminRole },
                new { UserName = "admin2",   Email = "admin2@test.com", Role = adminRole },

                new { UserName = "moderator1",   Email = "moderator1@test.com", Role = moderatorRole },
                new { UserName = "moderator2",   Email = "moderator2@test.com", Role = moderatorRole },
                new { UserName = "moderator3",   Email = "moderator3@test.com", Role = moderatorRole }
            };

            int userIndex = 1;

            foreach (var u in users)
            {
                if (await _userManager.FindByNameAsync(u.UserName) != null)
                   { continue; }

                User user = new User
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    EmailConfirmed = true
                };

                IdentityResult result = await _userManager.CreateAsync(user, $"Password{userIndex}!");
                userIndex++;

                if (result.Succeeded == false)
                {
                    throw new Exception(
                        $"Failed to create user {u.UserName}: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                result = await _userManager.AddToRoleAsync(user:user, role: u.Role);

                if (result.Succeeded == false)
                {
                    throw new Exception(
                        $"Failed to add user {u.UserName} to role {userRole}: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
