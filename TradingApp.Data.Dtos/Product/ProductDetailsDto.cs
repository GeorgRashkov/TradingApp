
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Dtos.Product
{
    public class ProductDetailsDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public ProductStatus Status { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime FirstSellOrderCreationDate { get; set; }

        public DateTime LastSellOrderCreationDate { get; set; }

        public int ActiveSellOrdersCount { get; set; }
    }
}
