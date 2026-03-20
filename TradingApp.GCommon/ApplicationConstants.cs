using TradingApp.GCommon.Enums;

namespace TradingApp.GCommon
{
    public static class ApplicationConstants
    {
        public const int InvoicesPerPage = 3;
        public const int ProductsPerPage = 4;
        public const int RequestsPerPage = 6;
        public const int UsersPerPage = 4;


        public const int UserMaxActiveSellOrders = 5;
        public const int UserMaxActiveOrderSuggetions = 10;
        public const int ProductMaxActiveSellOrders = 3;        

        public const string DateFormat = "dd/MM/yyyy";
        public const string DateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        
        public const ProductStatus CreatedProductDefaultStatus = ProductStatus.approved;
        public const SellOrderStatus CreatedSellOrderDefaultStatus = SellOrderStatus.active;
        public const OrderRequestStatus CreatedOrderRequestDefaultStatus = OrderRequestStatus.active;
    }
}
