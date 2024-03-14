using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Etammen.Controllers.Admin
{
    public class AdminController : Controller
    {
        // GET: AdminController
        public IActionResult Home()
        {
            return View();
        }

       
    }
}
