
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;
using TradingApp.GCommon.ErrorCodes;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class ProductReportOperationsService: IProductReportOperationsService
    {
        
        private readonly IProductReportRepository _productReportRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        public ProductReportOperationsService(IProductReportRepository productReportRepository, IProductRepository productRepository, IUserRepository userRepository)
        {
            _productReportRepository = productReportRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<Result> CreateReportAsync(string reporterId, Guid reportedProductId, string title, string message, ProductReportType reportType)
        {
            bool reporterExists = await _userRepository.DoesUserExistAsync(userId: reporterId);
            if (reporterExists == false)
            { return new Result(errorCode: UserErrorCodes.UserNotFound); }

            bool reportedProductExists = await _productRepository.DoesProductExistAsync(productId: reportedProductId);
            if (reportedProductExists == false)
            { return new Result(errorCode: ProductErrorCodes.ProductNotFound); }

            bool isReportedProductCreatedByReporter = await _productRepository.DoesProductCreatedByUserExistAsync(userId: reporterId, productId: reportedProductId);
            if (isReportedProductCreatedByReporter == true)
            { return new Result(errorCode: ProductReportErrorCodes.ProductReportInvalidCreator); }

            ProductReport productReport = new ProductReport()
            {
                ReporterId = reporterId,
                ReportedProductId = reportedProductId,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                Type = reportType,
                Status = ApplicationConstants.CreatedProductReportDefaultStatus
            };

            await _productReportRepository.CreateReportAsync(report: productReport);
            
            return new Result();
        }
        
        
        public async Task<Result> SetReportStatusAsync(Guid reportId, ProductReportStatus newReportStatus) 
        {
            ProductReport? report = await _productReportRepository.GetProductReportByIdAsync(reportId: reportId);

            if(report == null) 
            { return new Result(errorCode: ProductReportErrorCodes.ProductReportNotFound); }

            await _productReportRepository.SetReportStatusAsync(report: report, newReportStatus: newReportStatus);

            return new Result();
        }

    }
}
