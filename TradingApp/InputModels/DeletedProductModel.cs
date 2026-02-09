namespace TradingApp.InputModels
{
    public class DeletedProductModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set;} = null!;        
    }
}
