using LanguageExt;
using Microsoft.AspNetCore.Identity;
using OrderControlApp.Dto;
using OrderControlApp.Models;
using System.Diagnostics;

namespace OrderControlApp.Managers
{
    public class AccountManager
    {
        private readonly UserManager<User> _accountManager;
        private readonly SignInManager<User> _signInManager;

        public AccountManager(UserManager<User> accountManager, SignInManager<User> signInManager)
        {
            _accountManager = accountManager;
            _signInManager = signInManager;
        }

        public async Task<Either<string, User>> RegistrationAsync(CreateUserDto createUserData)
        {
            User newUser = new User
            {
                UserName = createUserData.Name
            };


            var result = await _accountManager.CreateAsync(newUser, createUserData.Password);

            if (!result.Succeeded)
            {
                return "Не удалось создать пользователя";
            }

            return newUser;
        }

        public async Task<bool> LoginAsync(CreateUserDto loginUserData)
        {
            var user = await _accountManager.FindByNameAsync(loginUserData.Name);
            
            if (user == null)
            {
                return false;
            }

            return await _accountManager.CheckPasswordAsync(user, loginUserData.Password);
        }
    }
}
