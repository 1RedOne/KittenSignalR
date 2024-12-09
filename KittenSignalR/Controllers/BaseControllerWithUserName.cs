using KittenSignalR.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PictureAuth.Models;
using PictureAuth.Repo;

namespace KittenSignalR.Controllers
{
    public class BaseControllerWithUserName : Controller
    {
        private IShoppingCartManager shoppingCartManager;
        private FoxUserManager FoxUserManager;
        private ILogger logger;

        public BaseControllerWithUserName(IShoppingCartManager shoppingCartManager, FoxUserManager foxUserManager, ILogger logger)
        {
            this.shoppingCartManager = shoppingCartManager;
            this.FoxUserManager = foxUserManager;
            this.logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check for the login cookie
            var userRecord = this.GetLoggedInUser(HttpContext.Request.Cookies);
            var userName = userRecord.Name;

            if (userName == "Guest")
            {
                userName = null;
            }

            var itemCount = this.shoppingCartManager.GetItemsCountByUser(userRecord);

            if (!string.IsNullOrEmpty(userName))
            {
                // Set the user's name in ViewData
                ViewData["UserName"] = userName;
                ViewData["UserId"] = userRecord.Id;
                ViewData["ItemCount"] = itemCount;
            }
            else
            {
                // Clear ViewData if not logged in
                ViewData["UserName"] = null;
            }

            base.OnActionExecuting(context);
        }

        public RegisterDto GetLoggedInUser(IRequestCookieCollection cookies)
        {
            var userCookie = cookies["auth_cookie"];
            logger?.LogInformation($"Got cookie value of {userCookie}");

            if (null == userCookie)
            {
                return new RegisterDto { Name = "Guest" };
            }
            var user = FoxUserManager.GetUserFromCookie(userCookie);
            return user;
        }

        public RegisterDto GetLoggedInUser() => this.GetLoggedInUser(HttpContext.Request.Cookies);
    }
}