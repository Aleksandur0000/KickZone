using KickZone.Models;
using KickZone.Models.ViewModels;
using KickZone.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KickZone.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseService _db;

        public HomeController(ILogger<HomeController> logger, DatabaseService db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index(int? categoryId, string? brand, string? search)
        {
            var vm = new CatalogViewModel
            {
                CategoryId = categoryId,
                Brand = brand,
                Search = search
            };

            try
            {
                vm.Shoes = _db.GetShoes(categoryId, brand, search);
                vm.Categories = _db.GetCategories();
                vm.Brands = _db.GetBrands();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load shoes from database");
                vm.Error = "Неуспешно зареждане на продуктите. Провери връзката с базата.";
            }

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
