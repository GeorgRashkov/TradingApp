using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TradingApp.GCommon;

namespace TradingApp.Data.Models
{
    public class User: IdentityUser
    {
       
        [MaxLength(EntityValidation.User.LockoutMessageMaxLength)]
        public string? LockoutMessage { get; set; }

        public virtual Balance Balance { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public virtual ICollection<SellOrder> SellOrders { get; set; } = new HashSet<SellOrder>();
        public virtual ICollection<OrderRequest> OrderRequests { get; set; } = new HashSet<OrderRequest>();
        public virtual ICollection<CompletedOrder> CompletedSellOrders { get; set; } = new HashSet<CompletedOrder>();
        public virtual ICollection<CompletedOrder> CompletedPurchaseOrders { get; set; } = new HashSet<CompletedOrder>();

        //These are the products reported by the user
        public virtual ICollection<ProductReport> ProductReports { get; set; } = new List<ProductReport>();
    }
}
