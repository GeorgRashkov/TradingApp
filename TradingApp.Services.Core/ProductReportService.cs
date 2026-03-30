
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TradingApp.Data;
using TradingApp.GCommon;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.ProductReport;

namespace TradingApp.Services.Core
{
    public class ProductReportService : IProductReportService
    {
        private ApplicationDbContext _context;
        private int _productReportsPerPage;
        public ProductReportService(ApplicationDbContext context) 
        {
            _productReportsPerPage = ApplicationConstants.ProductReportsPerPage;
            _context = context;
        }
        public int ProductReportPageIndex { get; set; }
        private void SetReportPage(int pageIndex, int reportsCount)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex * _productReportsPerPage >= reportsCount ? (int)Math.Ceiling((decimal)reportsCount / (decimal)_productReportsPerPage) - 1 : pageIndex;
            ProductReportPageIndex = pageIndex;
        }

        public async Task<List<ProductReportViewModel>> GetReportsAsync(int pageIndex)
        {
            int reportsCount = await _context
                .ProductReports
              .AsNoTracking()              
              .CountAsync();

            if (reportsCount == 0)
            { return new List<ProductReportViewModel>(); }

            SetReportPage(pageIndex, reportsCount);

            List<ProductReportViewModel> reports = await _context
                .ProductReports
                .AsNoTracking()               
                .Skip(ProductReportPageIndex * _productReportsPerPage).Take(_productReportsPerPage)
                .Select(pr => new ProductReportViewModel
                {
                    ReportId = pr.Id,                    
                    Title = pr.Title,
                    CreatedAt = pr.CreatedAt.ToString(ApplicationConstants.DateFormat, CultureInfo.InvariantCulture),
                    Type = pr.Type.ToString(),
                    Status = pr.Status.ToString()
                }).ToListAsync();

            return reports;

        }
        public async Task<List<ProductReportViewModel>> GetReportsForProductAsync(int pageIndex, Guid reportedProductId) 
        {
            int reportsForProductCount = await _context
                .ProductReports
              .AsNoTracking()
              .Where(pr => pr.ReportedProductId == reportedProductId)
              .CountAsync();

            if (reportsForProductCount == 0)
            { return new List<ProductReportViewModel>(); }

            SetReportPage(pageIndex, reportsForProductCount);

            List<ProductReportViewModel> reportsForProduct = await _context
                .ProductReports
                .AsNoTracking()
                .Where(pr => pr.ReportedProductId == reportedProductId)
                .Skip(ProductReportPageIndex * _productReportsPerPage).Take(_productReportsPerPage)
                .Select(pr => new ProductReportViewModel
                {
                    ReportId = pr.Id,                    
                    Title = pr.Title,
                    CreatedAt = pr.CreatedAt.ToString(ApplicationConstants.DateFormat, CultureInfo.InvariantCulture),
                    Type = pr.Type.ToString(),
                    Status = pr.Status.ToString()
                }).ToListAsync();

            return reportsForProduct;

        }


        public async Task<ProductReportDetailsViewModel?> GetProductReportAsync(Guid reportId) 
        {
            ProductReportDetailsViewModel? report = await _context
                .ProductReports
                .Include(pr => pr.Product)
                .Include(pr => pr.Reporter)
                .AsNoTracking()
                .Where(pr => pr.Id == reportId)
                .Select(pr => new ProductReportDetailsViewModel 
                {
                    ReportId = pr.Id,
                    Title = pr.Title,
                    CreatedAt = pr.CreatedAt.ToString(ApplicationConstants.DateTimeFormat, CultureInfo.InvariantCulture),
                    Type = pr.Type.ToString(),
                    Status = pr.Status.ToString(),
                    ReportedProductId = pr.Product.Id,
                    ReporterName = pr.Reporter.UserName,
                    Message = pr.Message,
                })
                .FirstOrDefaultAsync();

            return report;
        }
    }
}
