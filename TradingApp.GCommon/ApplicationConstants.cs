using TradingApp.GCommon.Enums;

namespace TradingApp.GCommon
{
    public static class ApplicationConstants
    {
        public const int ProductsPerPage = 4;
        public const int ProductsMaxActiveSellOrdersPerUser = 3;
        public const int ProductMaxActiveSellOrdersPerUser = 2;

        public const string DateFormat = "dd/MM/yyyy";
        public const ProductStatus CreatedProductDefaultStatus = ProductStatus.approved;
    }
}
