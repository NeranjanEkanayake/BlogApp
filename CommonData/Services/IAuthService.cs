using CommonData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommonData.Services
{
    public interface IAuthService
    {
        Task<Microsoft.AspNetCore.Identity.SignInResult> SignInAsync(string username, string password);
        Task LogoutAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;

        public AuthService(SignInManager<UserModel> signInManager, UserManager<UserModel> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<Microsoft.AspNetCore.Identity.SignInResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Microsoft.AspNetCore.Identity.SignInResult.Failed;
            }

            return await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public Task<Microsoft.AspNetCore.Identity.SignInResult> SignInAsync(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
