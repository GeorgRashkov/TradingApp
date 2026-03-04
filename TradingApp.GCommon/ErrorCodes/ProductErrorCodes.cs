
namespace TradingApp.GCommon.ErrorCodes
{
    public class ProductErrorCodes
    {
        public const string ProductNotFound = "Product.NotFound";
        public const string ProductWithSameNameAlreadyExists = "Product.NameAlreadyExists";
        public const string ProductHasActiveSaleOrders = "Product.NoActiveSaleOrders";
        public const string ProductInvalidCreator = "Product.InvalidCreator";
    }
}
