
using TradingApp.Data.Repository.Interfaces;

namespace TradingApp.Data.Repository
{
    public class OrderRequestRepository: IOrderRequestRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
