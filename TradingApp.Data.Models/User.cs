using Microsoft.AspNetCore.Identity;

namespace TradingApp.Data.Models
{
    public class User: IdentityUser
    {
        public virtual Balance Balance { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public virtual ICollection<SellOrder> SellOrders { get; set; } = new HashSet<SellOrder>();
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new HashSet<PurchaseOrder>();
        public virtual ICollection<CompletedOrder> CompletedSellOrders { get; set; } = new HashSet<CompletedOrder>();
        public virtual ICollection<CompletedOrder> CompletedPurchaseOrders { get; set; } = new HashSet<CompletedOrder>();
    }
}
