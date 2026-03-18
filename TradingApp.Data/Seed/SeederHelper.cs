
using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Models;
using TradingApp.GCommon;

namespace TradingApp.Data.Seed
{
    public class SeederHelper
    {
        private ApplicationDbContext _context;

        public SeederHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<User> GetUsersWithRoleUser()
        {
            IQueryable<User> users = _context
                .Users
                .AsNoTracking()
                .Where(u => u.UserName.ToLower().Contains(ApplicationRoles.Admin.ToLower()) == false
                && u.UserName.ToLower().Contains(ApplicationRoles.Moderator.ToLower()) == false);

            return users;

        }
    }
}
