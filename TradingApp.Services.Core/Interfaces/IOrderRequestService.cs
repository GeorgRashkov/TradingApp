

using TradingApp.ViewModels.InputOrderRequest;
using TradingApp.ViewModels.OrderRequest;
using TradingApp.ViewModels.Product;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IOrderRequestService
    {
        int RequestPageIndex { get; }
        Task<IEnumerable<OrderRequestsViewModel>> GetActiveRequestsAsync(int pageIndex);
        Task<OrderRequestViewModel?> GetDetailsForActiveRequestAsync(Guid requestId);
        Task<IEnumerable<MyOrderRequestsViewModel>> GetActiveRequestsCreatedByUserAsync(int pageIndex, string userId);
        Task<MyOrderRequestViewModel?> GetDetailsForActiveRequestCreatedByUserAsync(Guid requestId, string userId);
        Task<int> GetUserActiveRequestsCountAsync(string userId);

        Task<UpdatedOrderRequestModel?> GetUpdatedOrderRequestModelAsync(Guid orderRequestId);
    }
}
