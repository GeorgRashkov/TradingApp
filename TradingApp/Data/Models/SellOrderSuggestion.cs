using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradingApp.Data.Models
{
    [PrimaryKey(nameof(SellOrderId), nameof(PurchaseOrderId))]
    public class SellOrderSuggestion
    {
        //[ForeignKey(nameof(SellOrder))]
        [Required]
        public Guid SellOrderId { get; set; }

        //[ForeignKey(nameof(PurchaseOrder))]
        [Required]
        public Guid PurchaseOrderId { get; set; }



        public virtual SellOrder SellOrder { get; set; } = null!;
        public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
    }
}
