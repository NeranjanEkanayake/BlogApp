using CommonData.DTO;
using CommonData.Models;
using CommonData.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        public readonly IUserService _userService;
        public readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(IUserService userService, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUser()
        {
            var users = await _userService.GetAllUsersAsync();
            
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserModel>> RegisterUser(RegisterViewDTO registerViewDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new UserModel
            {
                UserName = registerViewDTO.UserName,
                Name = registerViewDTO.Name,
            };

            string role = "User";

            var result = await _userManager.CreateAsync(user, registerViewDTO.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);

                return Ok(new
                {
                    Message = "User Registered Successfully",
                    UserId = user.Id,
                    Username = user.UserName
                });
            }

            return BadRequest(result.Errors.Select(d => d.Description));
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userService.DeleteUser(id);

            if (user == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(user);
        }
    }
}
