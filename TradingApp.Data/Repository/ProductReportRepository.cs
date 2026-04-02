
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TradingApp.Data.Dtos.ProductReport;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Repository
{
    public class ProductReportRepository : IProductReportRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //<number methods
        public async Task<int> GetReportsCountAsync()
        {
            int reportsCount = await _context
                .ProductReports
              .AsNoTracking()
              .CountAsync();

            return reportsCount;
        }

        public async Task<int> GetReportsCountForProductAsync(Guid productId)
        {
            int reportsCount = await _context
                .ProductReports
              .AsNoTracking()
              .Where(pr => pr.ReportedProductId == productId)
              .CountAsync();

            return reportsCount;
        }
        //number methods>


        //<entity methods
        public async Task<ProductReport?> GetProductReportByIdAsync(Guid reportId)
        {
            ProductReport? productReport = await _context.ProductReports.FindAsync(reportId);
            return productReport;
        }


        //entity methods>

        //<dto methods
        public async Task<IEnumerable<ProductReportDto>> GetProductReportsAsync(int skipCount, int takeCount)
        {
            List<ProductReportDto> reports = await _context
               .ProductReports
               .AsNoTracking()
               .Skip(skipCount).Take(takeCount)
               .Select(pr => new ProductReportDto
               {
                   ReportId = pr.Id,
                   Title = pr.Title,
                   CreatedAt = pr.CreatedAt,
                   Type = pr.Type,
                   Status = pr.Status
               }).ToListAsync();

            return reports;
        }


        public async Task<IEnumerable<ProductReportDto>> GetReportsForProductAsync(int skipCount, int takeCount, Guid reportedProductId)
        {
            IEnumerable<ProductReportDto> reportsForProduct = await _context
               .ProductReports
               .AsNoTracking()
               .Where(pr => pr.ReportedProductId == reportedProductId)
               .Skip(skipCount).Take(takeCount)
               .Select(pr => new ProductReportDto
               {
                   ReportId = pr.Id,
                   Title = pr.Title,
                   CreatedAt = pr.CreatedAt,
                   Type = pr.Type,
                   Status = pr.Status
               }).ToListAsync();

            return reportsForProduct;
        }

        public async Task<ProductReportDetailsDto?> GetProductReportAsync(Guid reportId)
        {
            ProductReportDetailsDto? report = await _context
                .ProductReports
                .Include(pr => pr.Product)
                .Include(pr => pr.Reporter)
                .AsNoTracking()
                .Where(pr => pr.Id == reportId)
                .Select(pr => new ProductReportDetailsDto
                {
                    ReportId = pr.Id,
                    Title = pr.Title,
                    CreatedAt = pr.CreatedAt,
                    Type = pr.Type,
                    Status = pr.Status,
                    ReportedProductId = pr.Product.Id,
                    ReporterName = pr.Reporter.UserName,
                    Message = pr.Message,
                })
                .FirstOrDefaultAsync();

            return report;
        }
        //dto methods>

        //<operation methods
        public async Task CreateReportAsync(ProductReport report)
        {
            await _context.ProductReports.AddAsync(report);
            
            int affectedEntities = await _context.SaveChangesAsync();
            if (affectedEntities != 1)
            {
                throw new Exception("Failed to create product report.");
            }
        }

        public async Task SetReportStatusAsync(ProductReport report, ProductReportStatus newReportStatus)
        {
            _context.Attach<ProductReport>(report);
            report.Status = newReportStatus;

            int affectedEntities = await _context.SaveChangesAsync();
            if (affectedEntities != 1)
            {
                throw new Exception("Failed to change the status of a product report.");
            }
        }
        //operation methods>
    }
}
