using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestApp.Models;

namespace TestApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly StoreContextFactory _contextFactory;
        private readonly ILogger _logger;

        public AccountController(ILogger<CartController> logger)
        {
            _contextFactory = new StoreContextFactory();

            Debug.Assert(logger != null);
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            using (var context = _contextFactory.CreateDbContext(null))
            {
                if (ModelState.IsValid)
                {
                    var user = await context.Users.FirstOrDefaultAsync(u =>
                        u.LoginNickname == model.LoginNickname && u.Password == model.Password);
                    if (user != null)
                    {
                        await Authenticate(model.LoginNickname);

                        return RedirectToAction("List", "Equipment");
                    }

                    ModelState.AddModelError("", "Wrong login or password");
                }

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            using (var context = _contextFactory.CreateDbContext(null))
            {
                if (ModelState.IsValid)
                {
                    var user = await context.Users.FirstOrDefaultAsync(u => u.LoginNickname == model.LoginNickname);
                    if (user == null)
                    {
                        context.Users.Add(new User(model.LoginNickname, model.Password));
                        await context.SaveChangesAsync();

                        await Authenticate(model.LoginNickname);

                        return RedirectToAction("List", "Equipment");
                    }

                    ModelState.AddModelError("", "Wrong login or password");
                }

                return View(model);
            }
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("List", "Equipment");
        }
    }
}