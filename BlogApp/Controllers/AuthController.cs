using BlogApp.Models;
using BlogApp.Models.DTO;
using BlogApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(
            IUserService userService,
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewDTO login,string returnUrl = "/")
        {
            if(!ModelState.IsValid) return View(login);

            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(login);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, login.Password, false, false);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError("", "Invalid login");
            return View(login);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterUser()
        {
            return View("~/Views/User/RegisterUser.cshtml");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterViewDTO registerViewDTO)
        {            
            if (!ModelState.IsValid)
            {
                return View(registerViewDTO);
            }

            var user = new UserModel
            {
                UserName = registerViewDTO.UserName,
                Name = registerViewDTO.Name,
            };

            //var isAdmin = User.IsInRole("Admin");
            //if (!isAdmin)
            //{
            //    registerViewDTO.Role = "User"; // Prevent non-admins from registering as Admin
            //}
            string role = "User";
            if (User.IsInRole("Admin"))
            {
                // Admins can assign custom roles (e.g., via query parameter or hidden field)
                role = HttpContext.Request.Form["Role"].ToString() ?? "User"; // Fallback to "User"
            }

            var result = await _userManager.CreateAsync(user, registerViewDTO.Password);

            if (result.Succeeded)
            {
                // Step 3: Ensure the role exists in AspNetRoles
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                // Step 4: Assign the role (saves to AspNetUserRoles)
                await _userManager.AddToRoleAsync(user, role);

                // Optional: Auto-sign-in
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Login", "Auth");
            }

            // Handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("~/Views/User/RegisterUser.cshtml", registerViewDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Blog");
        }
    }
}
