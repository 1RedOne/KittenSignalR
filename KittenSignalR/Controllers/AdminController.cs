using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PictureAuth.Auth;

namespace CustomAuth.Controllers
{
    [RequiresPictureAuth]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}