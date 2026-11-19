using KickZone.Models;
using KickZone.Models.ViewModels;
using KickZone.Services;
using Microsoft.AspNetCore.Mvc;

namespace KickZone.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseService _db;

        public AccountController(DatabaseService db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.IsLoggedIn())
                return LocalOrHome(returnUrl);
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _db.FindUser(model.Username ?? "");
            if (user == null || !PasswordHasher.Verify(model.Password ?? "", user.PasswordHash))
            {
                model.Error = "Грешно потребителско име или парола.";
                return View(model);
            }

            HttpContext.SetUser(user.Id, user.Username, user.Role);
            return LocalOrHome(model.ReturnUrl);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_db.UsernameExists(model.Username))
            {
                model.Error = "Потребителското име вече е заето.";
                return View(model);
            }

            if (_db.EmailExists(model.Email))
            {
                model.Error = "Имейлът вече е регистриран.";
                return View(model);
            }

            var user = new UserModel
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = PasswordHasher.Hash(model.Password),
                FirstName = model.FirstName ?? "",
                LastName = model.LastName ?? "",
                Role = "user"
            };
            _db.CreateUser(user);

            var created = _db.FindUser(model.Username);
            if (created != null)
                HttpContext.SetUser(created.Id, created.Username, created.Role);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.ClearUser();
            return RedirectToAction("Index", "Home");
        }

        private IActionResult LocalOrHome(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }
    }
}
