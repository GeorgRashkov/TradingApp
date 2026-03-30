

using TradingApp.ViewModels.InputOrderRequest;
using TradingApp.ViewModels.OrderRequest;
using TradingApp.ViewModels.Product;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IOrderRequestService
    {
        int RequestPageIndex { get; }
        Task<IEnumerable<OrderRequestViewModel>> GetActiveRequestsAsync(int pageIndex);
        Task<OrderRequestDetailsViewModel?> GetDetailsForActiveRequestAsync(Guid requestId);
        Task<IEnumerable<MyOrderRequestViewModel>> GetActiveRequestsCreatedByUserAsync(int pageIndex, string userId);
        Task<MyOrderRequestDetailsViewModel?> GetDetailsForActiveRequestCreatedByUserAsync(Guid requestId, string userId);
        Task<int> GetUserActiveRequestsCountAsync(string userId);

        Task<UpdatedOrderRequestModel?> GetUpdatedOrderRequestModelAsync(Guid orderRequestId);
    }
}
