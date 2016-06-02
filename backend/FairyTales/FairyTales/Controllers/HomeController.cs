using System.Web.Mvc;
using FairyTales.Models;

namespace FairyTales.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            MainPageData mpData = DbManager.MainPagePopulateTales();
            return View(mpData);
        }
	}
}