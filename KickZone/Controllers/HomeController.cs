using KickZone.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic;

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
            

            List<ShoesModel> Shoes = new List<ShoesModel>();

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

            ShoesModel shoe2 = new ShoesModel
            {
                Id = 2,
                Name = "Nike Shox",
                Size = 42,
                Price = 140.00m,

                Description = "????? ?? ??????",
                ImageUrl = "Images/11111.jpg",
                Color = "Black and Purple",
                Brand = "Nike",
                CategoryId = 2

            };


            Shoes.Add(shoe1);

            return View(Shoes);
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
