
namespace TradingApp.Data.Dtos.OrderRequest
{
    public class OrderRequestDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal MaxPrice { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }  
        public string CreatorUserName { get; set; } = null!;
        public bool HasSuggestions { get; set; } 
    }
}
