using System.Web.Mvc;
using FairyTales.Models;
using Microsoft.AspNet.Identity;

namespace FairyTales.Controllers
{
    public class FairyTaleController : Controller
    {
        // GET: FairyTale
        public ActionResult Index(string path)
        {
            if (string.IsNullOrEmpty(path) || !DbManager.ValidatePathByPath(path))
                return PartialView("Error");

            var fairyTale = DbManager.GetTaleByPath(path);

            if (User.Identity.IsAuthenticated)
            {
                fairyTale.IsUserLiked = DbManager.IsUserLikedTaleWithId(fairyTale.Id, User.Identity.GetUserId());
                fairyTale.IsUserFavorited = DbManager.IsUserFavoritedTaleWithId(fairyTale.Id, User.Identity.GetUserId());
            }

            return View(fairyTale);
        }
    }
}
