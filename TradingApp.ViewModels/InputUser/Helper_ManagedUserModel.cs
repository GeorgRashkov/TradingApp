
namespace TradingApp.ViewModels.InputUser
{
    public class Helper_ManagedUserModel
    {
        public string UserName { get; set; } = null!;

        //must contain all possible roles (it doesn't matter if the user has them or not)
        public List<string> Roles { get; set; } = new List<string>();
    }
}
