
using TradingApp.GCommon;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IProductOperationsService
    {
        Task<Result> AddProductAsync(string name, string description, decimal price, string creatorId);
        Task<Result> UpdateProductAsync(Guid id, string name, string description, decimal price, string creatorId);

        Task<Result> DeleteProductAsync(Guid id, string creatorId);
    }
}
