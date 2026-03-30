
namespace TradingApp.ViewModels.OrderRequest
{
    public class MyOrderRequestDetailsViewModel: MyOrderRequestViewModel
    {
        public string Description { get; set; } = null!;
        public string CreationDate { get; set; } = null!;
        
        public bool HasSuggestions { get; set; }
    }
}
