
namespace TradingApp.Services.Result
{
    public class Result
    {
        public Result(string? ErrorCode = null) 
        {            
            if (ErrorCode is not null)
            {
                Success = false;
                this.ErrorCode = ErrorCode;
            }
        }
        public bool Success { get; private set; } = true;
        public string? ErrorCode { get; private set; } = null;
    }
}
