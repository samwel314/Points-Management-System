using Microsoft.AspNetCore.Mvc;

namespace Robi_App.Controllers
{
    public class StoreController : Controller
    {
        public StoreController()
        {
            
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
