using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestProject.Helpers;
using TestProject.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace TestProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [Route("Register")]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [Route("Register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Error");
                return View();
            }

            AppUser newUser = new AppUser()
            {
                Name = register.Name,
                Surname = register.Surname,
                UserName = register.UserName,
                Email = register.Email
            };

            if (newUser is null)
            {
                ModelState.AddModelError("", "User doesn't exists");
                return View();
            }

            IdentityResult result = await _userManager.CreateAsync(newUser, register.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return View();
                }
            }

            await _userManager.AddToRoleAsync(newUser ,Enums.Roles.Admin.ToString());

            await _signInManager.SignInAsync(newUser, false);

            return RedirectToAction("Index", "Home");
        }

        [Route("Login")]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Error");
                return View();
            }

            AppUser user = await _userManager.FindByEmailAsync(login.Email);

            if (user is null)
            {
                ModelState.AddModelError("", "User doesn't exists");
                return View();
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(user, login.Password, login.IsPersistent, true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is wrong");
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task CreateRoles()
        {
            foreach (var role in Enum.GetNames(typeof(Enums.Roles)))
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
