using CommonData.DTO;
using CommonData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommonData.Services
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUserAsync(string userName, string password, string name, string role);
        Task<IdentityResult> UpdateUserAsync(string userId, string userName, string password, string name);
        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<IdentityResult> AddToRoleAsync(string userId, string role);
        Task<IdentityResult> RemoveFromRoleAsync(string userId, string role);
        Task<IEnumerable<UserViewDTO>> GetAllUsersAsync();
        Task<UserModel> GetUserByIdAsync(string userId);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<bool> RoleExistsAsync(string role);
        Task<IdentityResult> CreateRoleAsync(string role);
    }
}
