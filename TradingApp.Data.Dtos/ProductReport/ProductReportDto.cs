
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Dtos.ProductReport
{
    public class ProductReportDto
    {
        public Guid ReportId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public ProductReportType Type { get; set; }
        public ProductReportStatus Status { get; set; }
    }
}
