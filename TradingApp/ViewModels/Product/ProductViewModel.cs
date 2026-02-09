namespace TradingApp.ViewModels.Product
{
    public class ProductViewModel: ProductsViewModel
    {      
        public string Description { get; set; } = string.Empty;
     
        public string FirstSellOrderCreationDate { get; set; } = string.Empty;

        public string LastSellOrderCreationDate { get; set; } = string.Empty;

        public int SellOrdersCount { get; set; }
    }
}
