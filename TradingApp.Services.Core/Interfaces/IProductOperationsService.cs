using TradingApp.Data.Models;
using TradingApp.GCommon.Enums;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IProductOperationsService
    {
        Task AddProductAsync(string name, string description, decimal price, string creatorId);
        Task UpdateProductAsync(Guid id, string name, string description, decimal price, string creatorId);

        Task DeleteProductAsync(Guid productId);
    }
}
