
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Dtos.Product
{
    public class Product_ManageProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
       public ProductStatus Status { get; set; }
    }
}
