
namespace TradingApp.GCommon.ErrorCodes
{
    public class OrderRequestErrorCodes
    {
        public const string RequestNotFound = "Request.NotFound";
        public const string RequestWithSameTitleAlreadyExists = "Request.TitleAlreadyExists";
        public const string RequestInvalidStatus = "Request.InvalidStatus";
        public const string RequestInvalidCreator = "Request.InvalidCreator";
        public const string RequestSuggestionSameCreator = "Request.SuggestionSameCreator";
    }
}
