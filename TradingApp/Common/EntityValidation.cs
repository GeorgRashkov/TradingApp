namespace TradingApp.Common
{
    public static class EntityValidation
    {
        public static class Balance
        {
            public const string AmountDbType = "decimal (12,4)"; 
        }

        public static class Product
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 100;

            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 1000;

            public const string PriceDbType = "decimal (10,4)"; 
        }

        
        public static class Order
        {
            public const int TitleMinLength = 2;
            public const int TitleMaxLength = 100;

            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 2000;

            public const string PriceDbType = "decimal (10,4)";

            public const string DateType = "datetime2";
        }
                
    }
}
