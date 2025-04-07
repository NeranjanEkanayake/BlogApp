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
}
