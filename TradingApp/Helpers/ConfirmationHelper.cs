namespace TradingApp.Helpers
{
    public class ConfirmationHelper
    {
        public string Message { get; set; } = null!;
        public string Controller { get; set; } = null!;
        public string Action { get; set; } = null!;
        public Guid ProductId { get; set; }
        public int? OrdersCount { get; set; }
    }
}
