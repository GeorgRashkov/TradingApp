namespace TradingApp.ViewModels.Order
{
    public class OrderViewModel
    {
        public string Message { get; set; } = null!;       
        public Guid ProductId { get; set; } 
        public int? OrdersCount { get; set; }
    }
}
