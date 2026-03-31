
using TradingApp.GCommon.Filters;

namespace TradingApp.ViewModels.Product
{
    public class ProductsViewModel
    {
        public IEnumerable<ProductViewModel> Products { get; set; } = null!;
        public ProductFilter? ProductFilter { get; set; }
        public int PageIndex { get; set; }
    }
}
