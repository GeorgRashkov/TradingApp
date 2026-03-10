
namespace TradingApp.ViewModels.OrderRequest
{
    public class MyOrderRequestViewModel: MyOrderRequestsViewModel
    {
        public string Description { get; set; } = null!;
        public string CreationDate { get; set; } = null!;
        
        public bool HasSuggestions { get; set; }
    }
}
