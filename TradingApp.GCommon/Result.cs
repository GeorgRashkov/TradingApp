
namespace TradingApp.GCommon
{
    public class Result
    {
        public Result(string? errorCode = null) 
        {            
            if (errorCode is not null)
            {
                Success = false;
                ErrorCode = errorCode;
            }
        }
        public bool Success { get; private set; } = true;
        public string ErrorCode { get; private set; } = "";
    }
}
