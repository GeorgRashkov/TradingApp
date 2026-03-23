
using TradingApp.GCommon.Enums;

namespace TradingApp.ViewModels.ProductReport
{
    public class ProductsReportsViewModel
    {
        public string ReporterId { get; set; } = null!;
        public Guid ReportedProductId { get; set; }

        public string Title { get; set; } = null!;
        public string CreatedAt { get; set; } = null!;

        public string Type { get; set; }
        public string Status { get; set; }
    }
}
