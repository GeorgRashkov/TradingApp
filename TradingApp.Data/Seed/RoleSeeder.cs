

using Microsoft.AspNetCore.Identity;
using TradingApp.GCommon;

namespace TradingApp.Data.Seed
{
    public class RoleSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleSeeder(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedAsync() 
        {
            string[] roleNames = { ApplicationRoles.Admin, ApplicationRoles.Moderator, ApplicationRoles.User };
            foreach (string roleName in roleNames)
            {
                if (await _roleManager.RoleExistsAsync(roleName) == false)
                {
                    IdentityRole role = new IdentityRole(roleName);
                    IdentityResult result = await _roleManager.CreateAsync(role);
                    
                    if (result.Succeeded == false)
                    {
                        throw new Exception(
                            $"Failed to create role {roleName}: " +
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }                
            }
        }
    }
}
