using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MULTİSHOPMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
