namespace TradingApp.ViewModels.Product
{
    public class ProductDetailsViewModel: ProductViewModel
    {      
        public string Description { get; set; } = string.Empty;
     
        public string FirstSellOrderCreationDate { get; set; } = string.Empty;

        public string LastSellOrderCreationDate { get; set; } = string.Empty;

        public int SellOrdersCount { get; set; }
    }
}
