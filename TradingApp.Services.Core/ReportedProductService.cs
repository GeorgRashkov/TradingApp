
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TradingApp.Data;
using TradingApp.GCommon;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.ProductReport;

namespace TradingApp.Services.Core
{
    public class ReportedProductService : IReportedProductService
    {
        private ApplicationDbContext _context;
        private int _productReportsPerPage;
        public ReportedProductService(ApplicationDbContext context) 
        {
            _productReportsPerPage = ApplicationConstants.ProductReportsPerPage;
            _context = context;
        }
        public int ProductReportPageIndex { get; set; }


        public async Task<List<ProductsReportsViewModel>> GetReportsAsync(int pageIndex)
        {
            int reportsCount = await _context
                .ReportedProducts
              .AsNoTracking()              
              .CountAsync();

            if (reportsCount == 0)
            { return new List<ProductsReportsViewModel>(); }

            SetReportedProductPage(pageIndex, reportsCount);

            List<ProductsReportsViewModel> reports = await _context
                .ReportedProducts
                .AsNoTracking()               
                .Skip(ProductReportPageIndex * _productReportsPerPage).Take(_productReportsPerPage)
                .Select(rp => new ProductsReportsViewModel
                {
                    ReporterId = rp.ReporterId,
                    ReportedProductId = rp.ReportedProductId,
                    Title = rp.Title,
                    CreatedAt = rp.CreatedAt.ToString(ApplicationConstants.DateFormat, CultureInfo.InvariantCulture),
                    Type = rp.Type.ToString(),
                    Status = rp.Status.ToString()
                }).ToListAsync();

            return reports;

        }
        public async Task<List<ProductsReportsViewModel>> GetReportsForProductAsync(int pageIndex, Guid reportedProductId) 
        {
            int reportsForProductCount = await _context
                .ReportedProducts
              .AsNoTracking()
              .Where(rp => rp.ReportedProductId == reportedProductId)
              .CountAsync();

            if (reportsForProductCount == 0)
            { return new List<ProductsReportsViewModel>(); }

            SetReportedProductPage(pageIndex, reportsForProductCount);

            List<ProductsReportsViewModel> reportsForProduct = await _context
                .ReportedProducts
                .AsNoTracking()
                .Where(rp => rp.ReportedProductId == reportedProductId)
                .Skip(ProductReportPageIndex * _productReportsPerPage).Take(_productReportsPerPage)
                .Select(rp => new ProductsReportsViewModel
                {
                    ReporterId = rp.ReporterId,
                    ReportedProductId = rp.ReportedProductId,
                    Title = rp.Title,
                    CreatedAt = rp.CreatedAt.ToString(ApplicationConstants.DateFormat, CultureInfo.InvariantCulture),
                    Type = rp.Type.ToString(),
                    Status = rp.Status.ToString()
                }).ToListAsync();

            return reportsForProduct;

        }

        private void SetReportedProductPage(int pageIndex, int reportsCount)
        {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex * _productReportsPerPage >= reportsCount ? (int)Math.Ceiling((decimal)reportsCount / (decimal)_productReportsPerPage) - 1 : pageIndex;
            ProductReportPageIndex = pageIndex;
        }
    }
}
