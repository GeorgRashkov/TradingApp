using TradingApp.Data;
using TradingApp.Data.Models;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class BalanceService: IBalanceService
    {
        private ApplicationDbContext _context;
       
        public BalanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetUserBalanceAsync(string userId)
        {
            Balance balance = await GetBalanceAsync(userId);
            return balance.Amount;
        }

        public async Task IncreaseUserBalanceAsync(string userId, decimal increasement)
        {
            Balance balance = await GetBalanceAsync(userId);

            balance.Amount += increasement;
            _context.SaveChangesAsync();
        }

        public async Task DecreaseUserBalanceAsync(string userId, decimal decreasement)
        {
            Balance balance = await GetBalanceAsync(userId);

            balance.Amount = balance.Amount >= decreasement ? balance.Amount - decreasement : 0;
            _context.SaveChangesAsync();
        }

        private async Task<Balance> GetBalanceAsync(string userId)
        {
            Balance balance = await _context.Balances.FindAsync(userId);

            if (balance == null)
            {
                throw new InvalidOperationException("Cannot get the balance of non existing user!");
            }

            return balance;
        }
    }
}
