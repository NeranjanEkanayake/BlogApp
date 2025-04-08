using CommonData.Models;
using CommonData.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData.ServiceClasses
{   
    public class AuthService : IAuthService
    {
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;

        public AuthService(SignInManager<UserModel> signInManager, UserManager<UserModel> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<UserModel> ValidateUserAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) { return null; }

            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            return passwordValid ? user : null;
        }

        public async Task<IList<string>> GetUserRole(UserModel user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<SignInResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        //public Task<SignInResult> SignInAsync(string username, string password)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
