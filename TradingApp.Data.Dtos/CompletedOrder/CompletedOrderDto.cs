
namespace TradingApp.Data.Dtos.CompletedOrder
{
    public class CompletedOrderDto
    {
        public Guid Id { get; set; }
        public string BuyerId { get; set; } = null!;
        public string TitleForBuyer { get; set; } = null!;
        public string TitleForSeller { get; set; } = null!;
        public DateTime CompletedAt { get; set; }
    }
}
