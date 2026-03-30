namespace TradingApp.ViewModels.Product
{
    public class MyProductDetailsViewModel: MyProductViewModel
    {        
        public string Description { get; set; } = string.Empty;    
        public int ActiveSellOrdersCount { get; set; }        
    }
}
