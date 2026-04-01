
using TradingApp.Data.Repository.Interfaces;

namespace TradingApp.Data.Repository
{
    public class SellOrderRepository: ISellOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public SellOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
