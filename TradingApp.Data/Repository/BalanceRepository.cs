
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;

namespace TradingApp.Data.Repository
{
    public class BalanceRepository: IBalanceRepository
    {
        private readonly ApplicationDbContext _context;
        public BalanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Balance> GetBalanceAsync(string userId)
        {
            Balance? balance = await _context.Balances.FindAsync(userId);

            if (balance == null)
            {
                throw new InvalidOperationException("Cannot get the balance of non existing user!");
            }

            return balance;
        }

        
        public async Task<decimal> GetBalanceAmountAsync(string userId)
        {
            Balance balance = await GetBalanceAsync(userId);
            return balance.Amount;
        }

        //<operation methods
        public async Task CreateBalanceAsync(Balance balance)
        {
            await _context.Balances.AddAsync(balance);
            int affectedEntities = await _context.SaveChangesAsync();

            if (affectedEntities != 1)
            {
                throw new Exception("Failed to create Balance.");
            }
        }
        
        public async Task IncreaseBalanceAsync(string userId, decimal increasement)
        {
            Balance balance = await GetBalanceAsync(userId);

            balance.Amount += increasement;
            int affectedEntities = await _context.SaveChangesAsync();

            if (affectedEntities != 1)
            {
                throw new Exception("Failed to increase Balance.");
            }
        }

        public async Task DecreaseBalanceAsync(string userId, decimal decreasement)
        {
            Balance balance = await GetBalanceAsync(userId);

            balance.Amount = balance.Amount >= decreasement ? balance.Amount - decreasement : 0;
            int affectedEntities = await _context.SaveChangesAsync();

            if (affectedEntities != 1)
            {
                throw new Exception("Failed to decrease Balance.");
            }
        }
        //operation methods>
    }
}
