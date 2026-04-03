
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Dtos.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public string CreatorName { get; set; } = null!;
        public ProductStatus Status { get; set; }
    }
}
