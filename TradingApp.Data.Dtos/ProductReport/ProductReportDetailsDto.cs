
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Dtos.ProductReport
{
    public class ProductReportDetailsDto
    {
        public Guid ReportId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public ProductReportType Type { get; set; }
        public ProductReportStatus Status { get; set; }

        public Guid ReportedProductId { get; set; }

        //this is the username of the user who reported the product
        public string ReporterName { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
