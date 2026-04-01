
using TradingApp.Data.Repository.Interfaces;

namespace TradingApp.Data.Repository
{
    public class ProductRepository: IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
