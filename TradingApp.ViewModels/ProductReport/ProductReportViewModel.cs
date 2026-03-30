
namespace TradingApp.ViewModels.ProductReport
{
    public class ProductReportViewModel
    {
        public Guid ReportId { get; set; }    
        public string Title { get; set; } = null!;
        public string CreatedAt { get; set; } = null!;

        public string Type { get; set; }
        public string Status { get; set; }
    }
}
