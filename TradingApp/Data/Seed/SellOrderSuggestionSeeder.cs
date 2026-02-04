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

            List<Guid> sellOrdersIds = await _context
                .SellOrders
                .AsNoTracking()
                .OrderBy(so => so.Id)
                .Select(so => so.Id)
                .Take(5)
                .ToListAsync();

            List<Guid> purchaseOrdersIds = await _context
                .PurchaseOrders
                .AsNoTracking()
                .OrderBy(so => so.Id)
                .Select(so => so.Id)
                .Take(5)
                .ToListAsync();

            List<SellOrderSuggestion> sellOrderSuggestions = new List<SellOrderSuggestion>();
            int sellOrderSuggestionsCount = sellOrdersIds.Count;

            for (int i = 0; i < sellOrderSuggestionsCount; i++)
            {
                SellOrderSuggestion sellOrderSuggestion = new SellOrderSuggestion()
                {
                    SellOrderId = sellOrdersIds[i],
                    PurchaseOrderId = purchaseOrdersIds[i],
                };
                sellOrderSuggestions.Add(sellOrderSuggestion);
            }

            await _context.SellOrderSuggestions.AddRangeAsync(sellOrderSuggestions);

            await _context.SaveChangesAsync();
        }
    }
}
