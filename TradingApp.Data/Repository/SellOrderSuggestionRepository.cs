
using TradingApp.Data.Repository.Interfaces;

namespace TradingApp.Data.Repository
{
    public class SellOrderSuggestionRepository: ISellOrderSuggestionRepository
    {
        private readonly ApplicationDbContext _context;
        public SellOrderSuggestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
