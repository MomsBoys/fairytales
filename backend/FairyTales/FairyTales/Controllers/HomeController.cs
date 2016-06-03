using System.Web.Mvc;

namespace FairyTales.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            var mpData = DbManager.MainPagePopulateTales();
            return View(mpData);
        }
	}
}