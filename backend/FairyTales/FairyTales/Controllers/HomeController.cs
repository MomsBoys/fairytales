using System.Web.Mvc;
using FairyTales.Models;
using Microsoft.AspNet.Identity;

namespace FairyTales.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/
        public ActionResult Index()
        {
            var mainPage = DbManager.MainPagePopulateTales();

            if (User.Identity.IsAuthenticated)
            {
                DbManager.PopulateUserLikesAndFavorites(ref mainPage.LatestTales, User.Identity.GetUserId());
                DbManager.PopulateUserLikesAndFavorites(ref mainPage.PopularTales, User.Identity.GetUserId());
            }

            return View(mainPage);
        }
	}
}