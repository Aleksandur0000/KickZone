using KickZone.Models.ViewModels;
using KickZone.Services;
using Microsoft.AspNetCore.Mvc;

namespace KickZone.Controllers
{
    [AdminOnly]
    public class AdminController : Controller
    {
        private readonly DatabaseService _db;

        public AdminController(DatabaseService db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(new AdminDashboardViewModel
            {
                UserCount = _db.CountUsers(),
                ShoeCount = _db.CountShoes(),
                OrderCount = _db.CountOrders(),
                RecentOrders = _db.GetRecentOrders(),
                RevenueByDay = _db.GetRevenueByDay()
            });
        }

        public IActionResult Shoes()
        {
            return View(_db.GetShoes());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("ShoeForm", new ShoeFormViewModel
            {
                Categories = _db.GetCategories()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ShoeFormViewModel model)
        {
            if (!IsValidShoe(model))
            {
                model.Categories = _db.GetCategories();
                model.Error ??= "Провери данните на продукта.";
                return View("ShoeForm", model);
            }

            _db.CreateShoe(model.Shoe);
            return RedirectToAction(nameof(Shoes));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var shoe = _db.GetShoe(id);
            if (shoe == null)
                return NotFound();

            return View("ShoeForm", new ShoeFormViewModel
            {
                Shoe = shoe,
                Categories = _db.GetCategories()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ShoeFormViewModel model)
        {
            if (!IsValidShoe(model) || model.Shoe.Id <= 0)
            {
                model.Categories = _db.GetCategories();
                model.Error ??= "Провери данните на продукта.";
                return View("ShoeForm", model);
            }

            _db.UpdateShoe(model.Shoe);
            return RedirectToAction(nameof(Shoes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _db.DeleteShoe(id);
            return RedirectToAction(nameof(Shoes));
        }

        private static bool IsValidShoe(ShoeFormViewModel model)
        {
            if (model.Shoe == null)
                return false;
            if (string.IsNullOrWhiteSpace(model.Shoe.Name) ||
                string.IsNullOrWhiteSpace(model.Shoe.Brand) ||
                string.IsNullOrWhiteSpace(model.Shoe.Color) ||
                model.Shoe.CategoryId <= 0 ||
                model.Shoe.Price < 0 ||
                model.Shoe.Stock < 0)
            {
                return false;
            }
            return true;
        }
    }
}
