
namespace TradingApp.ViewModels.OrderRequest
{
    public class OrderRequestViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string MaxPrice { get; set; } = null!;
    }
}
