
namespace TradingApp.ViewModels.ProductReport
{
    public class ProductReportViewModel: ProductsReportsViewModel
    {
        //this is the username of the user who reported the product
         public string ReporterName { get; set; } = null!;

        //this is the username of the user who created the reported product
        public string ReportedProductCreatorName { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
