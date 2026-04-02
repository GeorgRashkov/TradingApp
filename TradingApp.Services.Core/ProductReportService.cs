
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TradingApp.Data.Dtos.ProductReport;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.ProductReport;

namespace TradingApp.Services.Core
{
    public class ProductReportService : IProductReportService
    {
        
        
        private readonly IProductReportRepository _productReportRepository;
        private int _productReportsPerPage;
        public ProductReportService(IProductReportRepository productReportRepository) 
        {
            _productReportRepository = productReportRepository;
            _productReportsPerPage = ApplicationConstants.ProductReportsPerPage;
            
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
            int reportsCount = await _productReportRepository.GetReportsCountAsync();

            if (reportsCount == 0)
            { return new List<ProductReportViewModel>(); }

            SetReportPage(pageIndex, reportsCount);

            IEnumerable<ProductReportDto> reportsDtos = await _productReportRepository.GetProductReportsAsync(skipCount: ProductReportPageIndex * _productReportsPerPage, takeCount: _productReportsPerPage);
            List<ProductReportViewModel> reports = reportsDtos.Select(pr => new ProductReportViewModel
            {
                ReportId = pr.ReportId,
                Title = pr.Title,
                CreatedAt = pr.CreatedAt.ToString(ApplicationConstants.DateFormat, CultureInfo.InvariantCulture),
                Type = pr.Type.ToString(),
                Status = pr.Status.ToString()
            }).ToList();

            return reports;

        }
        public async Task<List<ProductReportViewModel>> GetReportsForProductAsync(int pageIndex, Guid reportedProductId) 
        {
            int reportsForProductCount = await _productReportRepository.GetReportsCountForProductAsync(productId:reportedProductId);

            if (reportsForProductCount == 0)
            { return new List<ProductReportViewModel>(); }

            SetReportPage(pageIndex, reportsForProductCount);

            IEnumerable<ProductReportDto> reportsForProductDtos = await _productReportRepository.GetReportsForProductAsync(skipCount: ProductReportPageIndex * _productReportsPerPage, takeCount: _productReportsPerPage, reportedProductId: reportedProductId);
            List<ProductReportViewModel> reportsForProduct = reportsForProductDtos.Select(pr => new ProductReportViewModel
             {
                 ReportId = pr.ReportId,
                 Title = pr.Title,
                 CreatedAt = pr.CreatedAt.ToString(ApplicationConstants.DateFormat, CultureInfo.InvariantCulture),
                 Type = pr.Type.ToString(),
                 Status = pr.Status.ToString()
             }).ToList();
                        
            return reportsForProduct;

        }


        public async Task<ProductReportDetailsViewModel?> GetProductReportAsync(Guid reportId) 
        {
            ProductReportDetailsDto ? reportDto = await _productReportRepository.GetProductReportAsync(reportId: reportId);
            if(reportDto == null) 
            { return null; }

            ProductReportDetailsViewModel report = new ProductReportDetailsViewModel
            {
                ReportId = reportDto.ReportId,
                Title = reportDto.Title,
                CreatedAt = reportDto.CreatedAt.ToString(ApplicationConstants.DateTimeFormat, CultureInfo.InvariantCulture),
                Type = reportDto.Type.ToString(),
                Status = reportDto.Status.ToString(),
                ReportedProductId = reportDto.ReportedProductId,
                ReporterName = reportDto.ReporterName,
                Message = reportDto.Message,
            };

            return report;
        }
    }
}
