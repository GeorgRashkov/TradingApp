using TradingApp.GCommon.ErrorCodes;

namespace TradingApp.Services.Errors
{
    public static class ProductErrors
    {
        public static Error ProductNotFound => new Error(code: ProductErrorCodes.ProductNotFound, message: "The product was not found in the DB!");
        public static Error ProductWithSameNameAlreadyExists => new Error(code: ProductErrorCodes.ProductWithSameNameAlreadyExists, message: "A product with the same name already exists!");
        public static Error ProductHasActiveSaleOrders => new Error(code: ProductErrorCodes.ProductHasActiveSaleOrders, message: "The selected product has no active sale orders!");
    }
}
