using System.Web.Mvc;
using FairyTales.Models;

namespace FairyTales.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/
        public ActionResult Index()
        {
            return View(DbManager.MainPagePopulateTales());
        }
	}
}