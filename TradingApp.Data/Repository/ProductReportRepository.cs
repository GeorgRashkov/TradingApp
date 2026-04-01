
using TradingApp.Data.Repository.Interfaces;

namespace TradingApp.Data.Repository
{
    public class ProductReportRepository: IProductReportRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
