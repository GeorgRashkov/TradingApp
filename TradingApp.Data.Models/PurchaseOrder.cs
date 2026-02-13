using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Models
{
    public class PurchaseOrder
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(EntityValidation.Order.TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(EntityValidation.Order.DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        [Column(TypeName = EntityValidation.Order.PriceDbType)]
        public decimal MaxPrice { get; set; }

        [Required]
        [Column(TypeName = EntityValidation.Order.DateType)]
        public DateTime CreatedAt { get; set; }

        [Required]
        public PurchaseOrderStatus Status { get; set; }

        [Required]
        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; } = null!;



        public virtual User Creator { get; set; } = null!;

        public virtual ICollection<SellOrderSuggestion> SellOrderSuggestions { get; set; } = new HashSet<SellOrderSuggestion>();
    }
}
