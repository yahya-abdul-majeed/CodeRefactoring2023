using Microsoft.AspNetCore.Mvc;

namespace CLabManager_Web.Areas.Admin.Controllers
{
    public class ComputerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
