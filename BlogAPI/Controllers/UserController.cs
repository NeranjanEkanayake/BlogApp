using BlogAPI.Configuration;
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
        private readonly IAuthService _authService;
        private readonly JwtTokenService _jwtTokenService;
        public readonly IUserService _userService;
        public readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(IUserService userService, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager,
            IAuthService authService, JwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _userService = userService;
            _roleManager = roleManager;
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewDTO loginViewDTO)
        {
            var user = await _authService.ValidateUserAsync(loginViewDTO.UserName, loginViewDTO.Password);
            if(user == null)
            {
                return Unauthorized();
            }

            var roles = await _authService.GetUserRole(user);
            var token = _jwtTokenService.GenerateTokenAsync(user, roles);

            return Ok(new { token });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUser()
        {
            var users = await _userService.GetAllUsersAsync();
            
            return Ok(users);
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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
