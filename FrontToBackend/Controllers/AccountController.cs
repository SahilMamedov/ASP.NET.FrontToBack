using FrontToBackend.Models;
using FrontToBackend.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace FrontToBackend.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole>_roleManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser>signInManager, RoleManager<IdentityRole>roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = new AppUser
            {
                Fullname = registerVM.FullName,
                UserName = registerVM.UserName,
                Email = registerVM.Email
            };
            IdentityResult result= await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                    return View(registerVM);
                }
            }
            await _signInManager.SignInAsync(user, true);
            return RedirectToAction("Index","Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid) return View();

            AppUser appUser=await _userManager.FindByIdAsync(login.Email)
           if(appUser == null)
            {
                ModelState.AddModelError("","Email veya password duzgun deyil");
                return View(login);
            }

           SignInResult result= await _signInManager.PasswordSignInAsync(appUser, login.Password, true, true);
            return View();
        }


    }
}
