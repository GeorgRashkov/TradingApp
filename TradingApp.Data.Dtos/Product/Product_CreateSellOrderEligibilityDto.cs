using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Dtos.Product
{
    public class Product_CreateSellOrderEligibilityDto
    {
        public string Name { get; set; } = null!;
        public ProductStatus Status { get; set; }
        public string CreatorId { get; set; } = null!;
        public int ActiveSellOrdersCount { get; set; }
    }
}
