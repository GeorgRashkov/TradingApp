using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingApp.Common;

namespace TradingApp.Data.Models
{
    public class CompletedOrder
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(EntityValidation.Order.TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [Column(TypeName = EntityValidation.Order.PriceDbType)]
        public decimal PricePaid { get; set; }

        [Required]
        [Column(TypeName = EntityValidation.Order.PriceDbType)]
        public decimal PlatformFee { get; set; }

        [Required]
        [Column(TypeName = EntityValidation.Order.PriceDbType)]
        public decimal SellerRevenue { get; set; }

        [Required]
        [Column(TypeName = EntityValidation.Order.DateType)]
        public DateTime CompletedAt { get; set; }

        
        public Guid? ProductId { get; set; }
        public string? BuyerId { get; set; }

        public string? SellerId { get; set; }
                
        

        public virtual Product? Product { get; set; }
        public virtual User? Buyer { get; set; }
        public virtual User? Seller { get; set; }

    }
}
