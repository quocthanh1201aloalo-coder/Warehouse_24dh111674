using System.Web.Mvc;

namespace Warehouse_24dh111674.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Products");
        }
    }
}
