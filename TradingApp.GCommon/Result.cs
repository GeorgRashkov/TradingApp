
namespace TradingApp.GCommon
{
    public class Result
    {
        public Result(string? errorCode = null, string? errorMessage = null, string? successMessage = null) 
        {            
            if (errorCode is not null)
            {
                Success = false;
                ErrorCode = errorCode;
                ErrorMessage = errorMessage;
            }
            else
            {
                SuccessMessage = successMessage;
            }
        }
        public bool Success { get; private set; } = true;
        public string ErrorCode { get; private set; } = "";
        public string? ErrorMessage { get; set; } = null;
        public string? SuccessMessage { get; set; } = null;
    }
}
