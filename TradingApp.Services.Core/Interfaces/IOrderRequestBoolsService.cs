
namespace TradingApp.Services.Core.Interfaces
{
    public interface IOrderRequestBoolsService
    {
        Task<bool> DoesOrderRequestExistAsync(Guid orderRequestId);
        Task<bool> DoesOrderRequestCreatedByUserExistAsync(string userId, string orderRequestTitle);
        Task<bool> IsOrderRequestActiveAsync(Guid orderRequestId);
        Task<bool> DoesOrderRequestCreatedByUserExistAsync(string userId, string orderRequestTitle, Guid[] orderRequestIdsToIgnore);
    }
}
