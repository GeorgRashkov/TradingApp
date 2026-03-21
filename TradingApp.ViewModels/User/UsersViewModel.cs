

namespace TradingApp.ViewModels.User
{
    public class UsersViewModel
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int DaysToSuspend { get; set; }       
        public bool IsAdmin { get; set; }
    }
}
