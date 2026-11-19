using KickZone.Services;
using Microsoft.AspNetCore.Mvc;

namespace KickZone.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DatabaseService _db;

        public ProductsController(DatabaseService db)
        {
            _db = db;
        }

        public IActionResult Details(int id)
        {
            var shoe = _db.GetShoe(id);
            if (shoe == null)
                return NotFound();

            return View(shoe);
        }
    }
}
