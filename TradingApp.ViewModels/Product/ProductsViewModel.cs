namespace TradingApp.ViewModels.Product
{
    public class ProductsViewModel
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;         
        public string Price { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string? Status { get; set; }
    }
}
