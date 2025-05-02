using CommonData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommonData.Services
{
    public interface IAuthService
    {
        Task<IList<string>> GetUserRole(UserModel user);
        Task LogoutAsync();
    }
}
