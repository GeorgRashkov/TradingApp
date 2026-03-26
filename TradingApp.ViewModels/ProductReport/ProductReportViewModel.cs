
namespace TradingApp.ViewModels.ProductReport
{
    public class ProductReportViewModel: ProductsReportsViewModel
    {
        public Guid ReportedProductId { get; set; }

        //this is the username of the user who reported the product
        public string ReporterName { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
