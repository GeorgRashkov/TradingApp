
using TradingApp.GCommon.Enums;

namespace TradingApp.ViewModels.InputProduct
{
    public class ManagedProductModel
    {
        public string? Name { get; set; }
        public Guid Id { get; set; }
        public ProductStatus Status { get; set; }
        public static List<ProductStatus> productStatuses { get; } = Enum
                .GetValues(typeof(ProductStatus))
                .Cast<ProductStatus>()
                .ToList();        
    }
}
