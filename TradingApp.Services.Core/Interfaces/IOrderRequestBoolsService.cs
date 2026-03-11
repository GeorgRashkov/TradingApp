
namespace TradingApp.Services.Core.Interfaces
{
    public interface IOrderRequestBoolsService
    {
        Task<bool> DoesOrderRequestExistAsync(Guid orderRequestId);
        Task<bool> IsOrderRequestActiveAsync(Guid orderRequestId);
    }
}
