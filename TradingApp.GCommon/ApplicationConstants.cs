using TradingApp.GCommon.Enums;

namespace TradingApp.GCommon
{
    public static class ApplicationConstants
    {
        public const int InvoicesPerPage = 3;
        public const int ProductsPerPage = 4;
        public const int UserMaxActiveSellOrders = 5;
        public const int ProductMaxActiveSellOrders = 3;

        public const string DateFormat = "dd/MM/yyyy";
        public const ProductStatus CreatedProductDefaultStatus = ProductStatus.approved;
        public const SellOrderStatus CreatedSellOrderDefaultStatus = SellOrderStatus.active;
    }
}
