namespace TradingApp.ViewModels.Invoice
{
    public class InvoiceDetailsViewModel: InvoiceViewModel
    {
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; } = null!;
        public string? ProductCreatorName { get; set; } = null!;

        public string Price { get; set; } = null!;

        public bool IsUserTheBuyer { get; set; }
    }
}
