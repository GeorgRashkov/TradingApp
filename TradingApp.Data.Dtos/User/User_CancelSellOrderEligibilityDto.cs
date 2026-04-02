
namespace TradingApp.Data.Dtos.User
{
    public class User_CancelSellOrderEligibilityDto
    {
        public string UserId { get; set; } = null!;
        public int ActiveSellOrdersCount { get; set; }
    }
}
