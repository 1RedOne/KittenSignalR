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

namespace CustomAuth.Controllers
{
    public class AccountController : Controller
    {
        private readonly FoxUserManager userManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(FoxUserManager userManager, ILogger<AccountController> _logger)
        {
            this.userManager = userManager;
            this.logger = _logger;
        }

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
                return RedirectToAction("Index");
            }
        }

        [HttpPut, AllowAnonymous]
        public async Task<IActionResult> RegisterMe()
        {
            using var reader = new StreamReader(this.Request.Body);
            var body2 = reader.ReadToEndAsync().Result;

            var register = JsonSerializer.Deserialize<RegisterDto>(body2);

            var createResult = await this.userManager.CreateAsync(register, new System.Threading.CancellationToken());

            this.HttpContext.Response.Cookies.Append("auth_cookie", register.AuthToken.ToString(), new CookieOptions
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
            RegisterDto user = this.GetLoggedInUser(this.HttpContext.Request.Cookies);

            return View(user);
        }

        private RegisterDto GetLoggedInUser(IRequestCookieCollection cookies)
        {
            var userCookie = cookies["auth_cookie"];
            this.logger?.LogInformation($"Got cookie value of {userCookie}");

            var user = this.userManager.GetUserFromCookie(userCookie);
            return user;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult SignIn()
        {
            string model = null;// new UserRegistrationDto();
            return View(model);
        }
    }
}