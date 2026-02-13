namespace TradingApp.ViewModels.Product
{
    public class MyProductsViewModel
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;        
        public string ProductStatus { get; set; } = string.Empty;        
    }
}
