using KickZone.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KickZone.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ShoesModel shoe1 = new ShoesModel
            {
                Id = 1,
                Name = "Nike Air Max",
                Size = 42,
                Price = 150.00m,

                Description = "????? ?? ??????",
                ImageUrl = "Images/11111.jpg",
                Color = "Neon Green",
                Brand = "Nike",
                CategoryId = 1

            };
            return View(shoe1);
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
