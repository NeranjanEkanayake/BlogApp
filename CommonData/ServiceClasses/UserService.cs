using CommonData.DTO;
using CommonData.Models;
using CommonData.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData.ServiceClasses
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> CreateUserAsync(string userName, string password, string name, string role)
        {
            var user = new UserModel
            {
                UserName = userName,
                Name = name,
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded && !string.IsNullOrEmpty(role))
            {
                await EnsureRoleExistsAsync(role);
                await _userManager.AddToRoleAsync(user, role);
            }
            return result;
        }

        public async Task<IdentityResult> UpdateUserAsync(string userId, string userName, string password, string name)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }
            user.UserName = userName;
            user.Name = name;

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<IdentityResult> AddToRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            await EnsureRoleExistsAsync(role);
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return await _userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task<IEnumerable<UserViewDTO>> GetAllUsersAsync()
        {
            return await _userManager.Users.Select(u => new UserViewDTO()
            {
                Id = u.Id,
                Name = u.Name,
                UserName = u.UserName
            }).ToListAsync();
        }        

        public async Task<UserViewDTO> GetUserByIdAsync(string userId)
        {
            var user =await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return null;
            }
            return new UserViewDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name
            };
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> RoleExistsAsync(string role)
        {
            return await _roleManager.RoleExistsAsync(role);
        }

        public async Task<IdentityResult> CreateRoleAsync(string role)
        {
            return await _roleManager.CreateAsync(new IdentityRole(role));
        }

        private async Task EnsureRoleExistsAsync(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        public async Task<IdentityResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            
            return await _userManager.DeleteAsync(user);
        }
    }
}
