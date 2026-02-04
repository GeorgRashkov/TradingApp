using Microsoft.AspNetCore.Identity;
using TradingApp.Data.Models;

namespace TradingApp.Data.Seed
{
    public class UserSeeder
    {
        private readonly UserManager<User> _userManager;

        public UserSeeder(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            var users = new[]
            {
                new { UserName = "alice",   Email = "alice@test.com" },
                new { UserName = "bob",     Email = "bob@test.com" },
                new { UserName = "charlie", Email = "charlie@test.com" },
                new { UserName = "diana",   Email = "diana@test.com" },
                new { UserName = "edward",  Email = "edward@test.com" },
                new { UserName = "frank",   Email = "frank@test.com" },
                new { UserName = "grace",   Email = "grace@test.com" }
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

                var result = await _userManager.CreateAsync(user, $"Password{userIndex}!");
                userIndex++;

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Failed to create user {u.UserName}: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
