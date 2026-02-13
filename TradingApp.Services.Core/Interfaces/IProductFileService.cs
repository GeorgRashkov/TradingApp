
using TradingApp.ViewModels.InputProduct;

namespace TradingApp.Services.Core.Interfaces
{
    public interface IProductFileService
    {
        Task SaveProductInFolderAsync(CreatedProductModel product, string creatorName);
        Task UpdateProductInFolderAsync(UpdatedProductModel product, string creatorName, string oldProductName);
        void DeleteProductFolder(string creatorName, string productName);
        public byte[] Get3dModelFileBytes(string creatorName, string productName);        
    }
}
