
namespace TradingApp.GCommon.ErrorCodes
{
    public class ProductErrorCodes
    {
        public const string ProductNotFound = "Product.NotFound";
        public const string ProductWithSameNameAlreadyExists = "Product.NameAlreadyExists";
        public const string ProductHasActiveSaleOrders = "Product.HasActiveSaleOrders";
        public const string ProductHasNonResolvedReports = "Product.HasNonResolvedReports";
        public const string ProductHasNoActiveSaleOrders = "Product.HasNoActiveSaleOrders";
        public const string ProductInvalidCreator = "Product.InvalidCreator";
        public const string ProductInvalidStatus = "Product.InvalidStatus";
        public const string ProductMaxActiveSellOrdersReached = "Product.MaxActiveSellOrdersReached";
        public const string ProductAlreadyPurchased = "Product.AlreadyPurchased";
        public const string ProductAlreadySuggestedToRequest = "Product.AlreadySuggestedToRequest";
    }
}
