using KittenSignalR.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using PictureAuth.Auth;
using PictureAuth.Models;
using PictureAuth.Repo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace KittenSignalR.Controllers
{
    public class AccountController : BaseControllerWithUserName
    {
        private readonly IShoppingCartManager shoppingCartManager;
        private readonly FoxUserManager userManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(FoxUserManager userManager, IShoppingCartManager shoppingCartManager, ILogger<AccountController> _logger) : base(shoppingCartManager, userManager, _logger)
        {
            this.userManager = userManager;
            logger = _logger;
        }

        [Route("[controller]")]
        [HttpGet, AllowAnonymous]
        public IActionResult Index()
        {
            // Check for the presence of the login cookie
            var loginCookie = HttpContext.Request.Cookies["auth_cookie"];

            if (!string.IsNullOrEmpty(loginCookie))
            {
                // If the cookie is present, redirect to the MyAccount view
                return RedirectToAction("MyAccount");
            }
            else
            {
                // If no cookie, redirect to the Index action
                return RedirectToAction("Register");
            }
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            List<RegisterDto> users = this.userManager.ListUsers();

            return View(users);
        }

        [HttpPut, AllowAnonymous]
        public async Task<IActionResult> LogMeIn()
        {
            using var reader = new StreamReader(Request.Body);
            var body2 = reader.ReadToEndAsync().Result;

            var loginAttempt = JsonSerializer.Deserialize<RegisterDto>(body2);

            var existingUser = userManager.FindByNameAsync(loginAttempt, new System.Threading.CancellationToken());
            await Task.Delay(50);

            if (null == existingUser)
            {
                var k = new UnauthorizedObjectResult("Invalid User Name");
                return Json(k);
            }

            if (!VerifyPassword(loginAttempt, existingUser))
            {
                var k = new UnauthorizedObjectResult("Invalid Password");
                return Json(k);
            }

            HttpContext.Response.Cookies.Append("auth_cookie", existingUser.AuthToken.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Json(new OkObjectResult("Logged in!"));
            //return RedirectToAction("MyAccount");
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> LogMeOut()
        {
            //delete the auth_cookie if present
            HttpContext.Response.Cookies.Delete("auth_cookie");

            return RedirectToAction("Index");
            
            //return RedirectToAction("MyAccount");
        }

        private bool VerifyPassword(RegisterDto loginAttempt, RegisterDto existingUser)
        {
            return loginAttempt.Password.Equals(existingUser.Password);
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPut, AllowAnonymous]
        public async Task<IActionResult> RegisterMe()
        {
            using var reader = new StreamReader(Request.Body);
            var body2 = reader.ReadToEndAsync().Result;

            var register = JsonSerializer.Deserialize<RegisterDto>(body2);

            var createResult = await userManager.CreateAsync(register, new System.Threading.CancellationToken());

            HttpContext.Response.Cookies.Append("auth_cookie", register.AuthToken.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return View("MyAccount", register);
        }

        [HttpGet, RequiresPictureAuth]
        public async Task<IActionResult> MyAccount()
        {
            RegisterDto user = GetLoggedInUser(HttpContext.Request.Cookies);

            if (user.Name == "Guest" || user.Name == null)
            {
                return RedirectToAction("Register");
            }
            return View(user);
        }

        [HttpGet, RequiresPictureAuth]
        public async Task<IActionResult> List()
        {
            RegisterDto user = GetLoggedInUser(HttpContext.Request.Cookies);

            if (user.IsAdmin())
            {
                List<RegisterDto> users = this.userManager.ListUsers();
                return View("AllAccounts", users);
            }

            return RedirectToAction("Register");
        }

        [HttpGet, AllowAnonymous]
        public IActionResult SignIn()
        {
            string model = null;// new UserRegistrationDto();
            return View(model);
        }
    }
}