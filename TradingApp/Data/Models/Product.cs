using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingApp.Common;
using TradingApp.Data.Enums;

namespace TradingApp.Data.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(EntityValidation.Product.NameMaxLength)]
        public string Name { get; set; } = null!;


        [Required]
        [MaxLength(EntityValidation.Product.DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        [Column(TypeName = EntityValidation.Product.PriceDbType)]
        public decimal Price { get; set; }

        [Required]
        public ProductStatus Status { get; set; }

        [Required]
        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; } = null!;



        public virtual IdentityUser Creator { get; set; } = null!;
        public virtual ICollection<CompletedOrder> CompletedOrders { get; set; } = new HashSet<CompletedOrder>();
        public virtual ICollection<SellOrder> SellOrders { get; set; } = new HashSet<SellOrder>();
    }
}
