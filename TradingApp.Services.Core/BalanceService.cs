using Microsoft.AspNetCore.Identity;
using TradingApp.Data.Models;
using TradingApp.Data.Repository.Interfaces;
using TradingApp.Services.Core.Interfaces;

namespace TradingApp.Services.Core
{
    public class BalanceService: IBalanceService
    {
        
        private IBalanceRepository _balanceRepository;
        private UserManager<User> _userManager;

        public BalanceService(IBalanceRepository balanceRepository, UserManager<User> userManager)
        {           
            _balanceRepository = balanceRepository;
            _userManager = userManager;
        }

        public async Task CreateBalanceAsync(string userId)
        {
            User? user = await _userManager.FindByIdAsync(userId: userId);

            if (user == null) 
            {
                throw new InvalidOperationException("Cannot create a balance for non existing user!");
            }

            Balance balance = new Balance()
            {
                Id = userId,
                Amount = 0,
            };

            await _balanceRepository.CreateBalanceAsync(balance: balance);           
        }


        public async Task<decimal> GetUserBalanceAsync(string userId)
        {
            return await _balanceRepository.GetBalanceAmountAsync(userId: userId);
        }

        public async Task IncreaseUserBalanceAsync(string userId, decimal increasement)
        {
            await _balanceRepository.IncreaseBalanceAsync(userId: userId, increasement: increasement);
        }

        public async Task DecreaseUserBalanceAsync(string userId, decimal decreasement)
        {
            await _balanceRepository.DecreaseBalanceAsync(userId: userId, decreasement: decreasement);
        }
    }
}
