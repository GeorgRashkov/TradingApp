using TradingApp.GCommon.ErrorCodes;

namespace TradingApp.Services.Errors
{
    public class UserErrors
    {
        public Error UserNotFound => new Error(code: UserErrorCodes.UserNotFound, message: "The user was not found in the DB!");
    }
}
