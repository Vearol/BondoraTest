using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data.Interfaces;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestApp.Models;

namespace TestApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public AccountController(ILogger<CartController> logger, IUserRepository userRepository)
        {
            _userRepository = userRepository;

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
            if (ModelState.IsValid)
            {
                var user = _userRepository.GetBy(u => u.LoginNickname == model.LoginNickname && u.Password == model.Password).FirstOrDefault();
                if (user != null)
                {
                    await Authenticate(model.LoginNickname);

                    _logger.LogInformation($"User with login:{model.LoginNickname} has just logged in.");

                    return RedirectToAction("List", "Equipment");
                }

                ModelState.AddModelError("", "Wrong login or password");
            }

            return View(model);
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
            if (ModelState.IsValid)
            {
                var user = _userRepository.GetBy(u => u.LoginNickname == model.LoginNickname && u.Password == model.Password).FirstOrDefault();
                if (user == null)
                {
                    _userRepository.Create(new User(model.LoginNickname, model.Password));
                    _userRepository.Save();

                    await Authenticate(model.LoginNickname);

                    _logger.LogInformation($"Registered new user. login:{model.LoginNickname}.");

                    return RedirectToAction("List", "Equipment");
                }

                ModelState.AddModelError("", "Wrong login or password");
            }

            return View(model);
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

            _logger.LogInformation($"User with login:{User.Identity.Name} has just logged out.");

            return RedirectToAction("List", "Equipment");
        }
    }
}