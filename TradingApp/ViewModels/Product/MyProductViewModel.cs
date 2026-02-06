using TradingApp.Data.Enums;

namespace TradingApp.ViewModels.Product
{
    public class MyProductViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;

        public bool HasActiveSellOrder { get; set; }        
    }
}
