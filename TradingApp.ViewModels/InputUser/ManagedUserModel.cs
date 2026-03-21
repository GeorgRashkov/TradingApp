
using System.ComponentModel.DataAnnotations;
using TradingApp.GCommon;

namespace TradingApp.ViewModels.InputUser
{
    public class ManagedUserModel
    {
        public string UserId { get; set; } = null!;
        public string Role { get; set; } = null!;

        [Range(minimum:EntityValidation.User.MinDaysToSuspendUser,maximum: EntityValidation.User.MaxDaysToSuspendUser)]
        public int DaysToSuspend { get; set; }
        public string? LockoutMessage { get; set; }

        public Helper_ManagedUserModel? UserHelper {  get; set; }
    }
}
