
using TradingApp.Data.Repository.Interfaces;

namespace TradingApp.Data.Repository
{
    public class CompletedOrderRepository: ICompletedOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public CompletedOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
