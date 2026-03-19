namespace TradingApp.Helpers
{
    public class PaginationHelper
    {
        public int PageIndex { get; set; }

        public string? Area { get; set; }

        public string Controller { get; set; } = null!;

        public string Action { get; set; } = null!;

    }
}
