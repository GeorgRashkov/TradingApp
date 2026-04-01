
using TradingApp.Data.Repository.Interfaces;

namespace TradingApp.Data.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
