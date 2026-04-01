
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
    }
}
