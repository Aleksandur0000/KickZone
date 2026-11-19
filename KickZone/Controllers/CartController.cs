using KickZone.Models.ViewModels;
using KickZone.Services;
using Microsoft.AspNetCore.Mvc;

namespace KickZone.Controllers
{
    public class CartController : Controller
    {
        private readonly DatabaseService _db;

        public CartController(DatabaseService db)
        {
            _db = db;
        }

        private int? UserId => HttpContext.GetUserId();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(int id, int quantity = 1)
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account",
                    new { returnUrl = Url.Action("Details", "Products", new { id }) });

            if (quantity < 1)
                quantity = 1;

            var shoe = _db.GetShoe(id);
            if (shoe == null)
                return RedirectToAction("Index", "Home");

            if (shoe.Stock < quantity)
            {
                TempData["Error"] = "Няма достатъчно наличност.";
                return RedirectToAction("Details", "Products", new { id });
            }

            _db.AddToCart(UserId.Value, id, quantity);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Index()
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index)) });

            var items = _db.GetCart(UserId.Value);
            return View(new CartViewModel
            {
                Items = items,
                Total = items.Sum(i => i.ShoePrice * i.Quantity),
                Error = TempData["Error"] as string
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");

            _db.RemoveFromCart(id, UserId.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout()
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index)) });

            var items = _db.GetCart(UserId.Value);
            if (items.Count == 0)
                return RedirectToAction(nameof(Index));

            if (items.Any(i => i.Quantity > i.Stock))
            {
                TempData["Error"] = "Някои продукти нямат достатъчно наличност.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var orderId = _db.CreateOrder(UserId.Value, items);
                return RedirectToAction(nameof(Confirmation), new { id = orderId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Confirmation(int id)
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");

            ViewBag.OrderId = id;
            return View();
        }
    }
}
