using EbayChat.Models.DTOs;
using EbayChat.Services;
using Microsoft.AspNetCore.Mvc;

namespace EbayChat.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserServices _userServices;
        public AuthController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserLoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                // validation failed
                return View(model);
            }

            // check user from service
            var user = _userServices.GetUserByUsernameAndPassword(model.Username, model.Password).Result;
            if (user != null)
            {
                HttpContext.Session.SetString("username", user.username);
                HttpContext.Session.SetInt32("userId", user.id);
                HttpContext.Session.SetString("role", user.role);
                HttpContext.Session.SetString("avatarURL", user.avatarURL);

                return Redirect("/");
            }

            // login failed
            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
    }
}
