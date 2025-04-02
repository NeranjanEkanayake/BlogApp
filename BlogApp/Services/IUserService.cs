using Microsoft.AspNetCore.Identity;

namespace BlogApp.Services
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUserAsync(string userName, string password);
    }
}
