using TradingApp.Data.Enums;

namespace TradingApp.ViewModels.Product
{
    public class MyProductViewModel: MyProductsViewModel
    {        
        public string Description { get; set; } = string.Empty;    
        public int ActiveSellOrdersCount { get; set; }        
    }
}
