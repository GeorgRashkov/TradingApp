namespace TradingApp.ViewModels.Invoice
{
    public class InvoicesViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string CompletedAt { get; set; } = null!;
    }
}
