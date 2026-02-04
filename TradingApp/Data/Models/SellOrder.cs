using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingApp.Common;
using TradingApp.Data.Enums;

namespace TradingApp.Data.Models
{
    public class SellOrder
    {

        [Key]
        public Guid Id { get; set; }
           
        [Required]
        public SellOrderStatus Status { get; set; }

        [Required]
        [Column(TypeName = EntityValidation.Order.DateType)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; } = null!;

        [Required]
        //[ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; }



        public virtual User Creator { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;

        public virtual ICollection<SellOrderSuggestion> SellOrderSuggestions { get; set; } = new HashSet<SellOrderSuggestion>();
       
    }
}
