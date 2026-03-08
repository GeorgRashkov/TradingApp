using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TradingApp.Data.Models
{
    [PrimaryKey(nameof(SellOrderId), nameof(OrderRequestId))]
    public class SellOrderSuggestion
    {
        //[ForeignKey(nameof(SellOrder))]
        [Required]
        public Guid SellOrderId { get; set; }

        //[ForeignKey(nameof(PurchaseOrder))]
        [Required]
        public Guid OrderRequestId { get; set; }



        public virtual SellOrder SellOrder { get; set; } = null!;
        public virtual OrderRequest OrderRequest { get; set; } = null!;
    }
}
