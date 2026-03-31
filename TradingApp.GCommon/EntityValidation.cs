namespace TradingApp.GCommon
{
    public static class EntityValidation
    {
        public static class User
        {
            public const int LockoutMessageMinLength = 5;
            public const int LockoutMessageMaxLength = 300;

            public const int MinDaysToSuspendUser = 0;
            public const int MaxDaysToSuspendUser = 9999;

            public const int LockoutDefaultMinutes = 1;
            public const int MaxFailedAccessAttempts = 5;

            public const string UserNameAllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            public const string UserNameRegex = @"^[a-zA-Z0-9]+$";
            public const int UserNameMinLength = 3;
            public const int UserNameMaxLength = 50;

            public const int PasswordMinLength = 7;
            public const int PasswordMaxLength = 100;                      
        }

        public static class Balance
        {
            public const string AmountDbType = "decimal (12,4)";
        }

        public static class Product
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;
            public const string NameRegex = @"^[a-zA-Z0-9 ]*$";

            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 1000;

            public const double PriceMinValue = 0.1;
            public const double PriceMaxValue = 999_999;

            public const string PriceDbType = "decimal (10,4)";
        }


        public static class Order
        {
            public const int TitleMinLength = 2;
            public const int TitleMaxLength = 100;

            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 2000;

            public const double PriceMinValue = 0.1;
            public const double PriceMaxValue = 999_999;

            public const string PriceDbType = "decimal (10,4)";

            public const string DateType = "datetime2";
        }

        public static class ProductReport
        {
            public const int TitleMinLength = 5;
            public const int TitleMaxLength = 100;

            public const int MessageMinLength = 10;
            public const int MessageMaxLength = 2000;

            public const string DateType = "datetime2";
        }
                
    }
}
