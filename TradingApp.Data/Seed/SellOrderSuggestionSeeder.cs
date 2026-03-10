using Microsoft.EntityFrameworkCore;
using TradingApp.Data.Models;

namespace TradingApp.Data.Seed
{
    public class SellOrderSuggestionSeeder
    {
        private ApplicationDbContext _context;
        public SellOrderSuggestionSeeder(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.SellOrderSuggestions.AnyAsync()) 
            { return; }

            List<Guid> productsIds = await _context
                .Products
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Select(p => p.Id)
                .Take(5)
                .ToListAsync();

            List<Guid> orderRequestsIds = await _context
                .OrderRequests
                .AsNoTracking()
                .OrderBy(or => or.Id)
                .Select(or => or.Id)
                .Take(5)
                .ToListAsync();

            List<SellOrderSuggestion> sellOrderSuggestions = new List<SellOrderSuggestion>();
            int sellOrderSuggestionsCount = productsIds.Count;

            for (int i = 0; i < sellOrderSuggestionsCount; i++)
            {
                SellOrderSuggestion sellOrderSuggestion = new SellOrderSuggestion()
                {
                    ProductId = productsIds[i],
                    OrderRequestId = orderRequestsIds[i],
                };
                sellOrderSuggestions.Add(sellOrderSuggestion);
            }

            await _context.SellOrderSuggestions.AddRangeAsync(sellOrderSuggestions);

            await _context.SaveChangesAsync();
        }
    }
}
