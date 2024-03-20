using Microsoft.AspNetCore.Mvc;

namespace Etammen.Controllers
{
    public class MapsAPIController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
