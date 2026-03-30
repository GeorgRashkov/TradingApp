
namespace TradingApp.ViewModels.OrderRequest
{
    public class OrderRequestDetailsViewModel : OrderRequestViewModel
    {
        public string Description { get; set; } = null!;
        public string CreationDate { get; set; } = null!;
        public string CreatorName { get; set; } = null!;
    }
}
