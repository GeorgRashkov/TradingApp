using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Models;

namespace TradingApp.Data.Seed
{
    public class BalanceSeeder
    {
        private ApplicationDbContext _context;
        public BalanceSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.Balances.AnyAsync())
            { return; }

            decimal initialBalanceAmount = 1000m;

            var userIds = await _context
                .Users
                .AsNoTracking()
                .Select(u => u.Id)
                .ToListAsync();

            List<Balance> balances = new List<Balance>();

            foreach (var userId in userIds)
            {
                Balance balance = new Balance
                {
                    Id = userId,
                    Amount = initialBalanceAmount
                };
                balances.Add(balance);
            }

            await _context.Balances.AddRangeAsync(balances);
            await _context.SaveChangesAsync();
        }
    }
}
