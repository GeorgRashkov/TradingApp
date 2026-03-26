
using Microsoft.EntityFrameworkCore;
using TradingApp.Data;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.Data.Models;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class ReportedProductOperationsService: IReportedProductOperationsService
    {
        private ApplicationDbContext _context;
        public ReportedProductOperationsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> CreateReportAsync(string reporterId, Guid reportedProductId, string title, string message, ProductReportType reportType)
        {
            bool reporterExists = await _context.Users.AnyAsync(x => x.Id == reporterId);
            if (reporterExists == false)
            { return new Result(errorCode: UserErrorCodes.UserNotFound); }

            bool reportedProductExists = await _context.Products.AnyAsync(x => x.Id == reportedProductId);
            if (reportedProductExists == false)
            { return new Result(errorCode: ProductErrorCodes.ProductNotFound); }

            bool isReportedProductCreatedByReporter = await _context.Products.AnyAsync(x => x.Id == reportedProductId && x.CreatorId == reporterId);
            if (isReportedProductCreatedByReporter == true)
            { return new Result(errorCode: ProductReportErrorCodes.ProductReportInvalidCreator); }

            ReportedProduct reportedProduct = new ReportedProduct()
            {
                ReporterId = reporterId,
                ReportedProductId = reportedProductId,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                Type = reportType,
                Status = ApplicationConstants.CreatedProductReportDefaultStatus
            };

            await _context.ReportedProducts.AddAsync(reportedProduct);
            await _context.SaveChangesAsync();

            return new Result();
        }
        
        
        public async Task<Result> SetReportStatusAsync(Guid reportId, ProductReportStatus newReportStatus) 
        {
            ReportedProduct? report = await _context.ReportedProducts.FindAsync(reportId);

            if(report == null) 
            { return new Result(errorCode: ProductReportErrorCodes.ProductReportNotFound); }

            report.Status = newReportStatus;
            await _context.SaveChangesAsync();

            return new Result();
        }

    }
}
