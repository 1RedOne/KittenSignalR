using KittenSignalR.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PictureAuth.Auth;
using PictureAuth.Repo;
using System.Linq;

namespace KittenSignalR.Controllers
{
    public class ShoppingCartController : BaseControllerWithUserName
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly IShoppingCartManager shoppingCartManager;
        private readonly IVideoSourceManager videoSourceManager;

        public ShoppingCartController(IShoppingCartManager shoppingCartManager, FoxUserManager userManager, IVideoSourceManager videoSourceManager, ILogger<ShoppingCartController> _logger) : base(shoppingCartManager, userManager, _logger)
        {
            this._logger = _logger;
            this.shoppingCartManager = shoppingCartManager;
            this.videoSourceManager = videoSourceManager;
        }

        // GET: ShoppingCartController
        [Route("[controller]")]
        [HttpGet, AllowAnonymous]
        public IActionResult Index()
        {
            // Check for the presence of the login cookie
            var loginCookie = HttpContext.Request.Cookies["auth_cookie"];

            if (!string.IsNullOrEmpty(loginCookie))
            {
                // If the cookie is present, redirect to the MyAccount view
                return RedirectToAction("MyCart");
            }
            else
            {
                // If no cookie, redirect to the Index action
                return RedirectToAction("Register", "AccountsController");
            }
        }

        [Route("/cart")]
        [HttpGet, RequiresPictureAuth]
        public ActionResult MyCart()
        {
            var user = this.GetLoggedInUser();
            var thisUsersCart = this.shoppingCartManager.GetCartByUser(user);

            return View(thisUsersCart);
        }

        [Route("/cart/addToCart/{videoUri}")]
        [HttpPut, RequiresPictureAuth]
        public ActionResult AddToCart2(string videoUri) => this.AddToCart(videoUri);

        [Route("/cart/add/{videoUri}")]
        [HttpPut, RequiresPictureAuth]
        public ActionResult AddToCart(string videoUri)
        {
            var user = this.GetLoggedInUser();
            var thisUsersCart = this.shoppingCartManager.GetCartByUser(user);

            var desiredVideo = this.videoSourceManager.GetVideo(videoUri);

            if (null == desiredVideo)
            {
                this._logger?.LogInformation("Could not find this video");
                return Json(new OkResult());
            }

            this._logger?.LogInformation("Found the video, adding");

            thisUsersCart.SafeAddToCart(desiredVideo);

            this.shoppingCartManager.RefreshCart(thisUsersCart);

            return Json(new OkResult());
        }

        [Route("/cart/removeFromCart/{videoUri}")]
        [HttpPut, RequiresPictureAuth]
        public ActionResult RemoveFromCart(string videoUri)
        {
            var user = this.GetLoggedInUser();
            var thisUsersCart = this.shoppingCartManager.GetCartByUser(user);

            var desiredVideo = this.videoSourceManager.GetVideo(videoUri);

            if (null == desiredVideo)
            {
                this._logger?.LogInformation("Could not find this video");

                thisUsersCart.SafeDeleteByVideoUri(videoUri);
                return Json(new OkResult());
            }

            this._logger?.LogInformation("Found the video, adding");

            thisUsersCart.SafeDelete(desiredVideo);

            this.shoppingCartManager.RefreshCart(thisUsersCart);

            return Json(new OkResult());
        }

        // GET: ShoppinCartController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShoppinCartController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ShoppinCartController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ShoppinCartController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ShoppinCartController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ShoppinCartController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}