﻿using CommonData.Services;
using CommonData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CommonData.DTO;

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

            string role = "User";
            if (User.IsInRole("Admin"))
            {
                role = HttpContext.Request.Form["Role"].ToString() ?? "User";
            }

            var result = await _userManager.CreateAsync(user, registerViewDTO.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                await _userManager.AddToRoleAsync(user, role);

                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Login", "Auth");
            }

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
