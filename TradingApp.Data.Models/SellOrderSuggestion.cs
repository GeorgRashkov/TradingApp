using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TradingApp.Data.Models
{
    [PrimaryKey(nameof(ProductId), nameof(OrderRequestId))]
    public class SellOrderSuggestion
    {
        //[ForeignKey(nameof(Product))]
        [Required]
        public Guid ProductId { get; set; }

        //[ForeignKey(nameof(OrderRequest))]
        [Required]
        public Guid OrderRequestId { get; set; }



        public virtual Product Product { get; set; } = null!;
        public virtual OrderRequest OrderRequest { get; set; } = null!;
    }
}
