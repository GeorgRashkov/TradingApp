
namespace TradingApp.Data.Dtos.OrderRequest
{
    public class OrderRequestDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal MaxPrice { get; set; }
    }
}
