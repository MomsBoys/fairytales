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
            if (string.IsNullOrEmpty(path) || !DbManager.ValidateTaleByPath(path))
                return PartialView("Error");

            var fairyTale = DbManager.GetTaleByPath(path);

            if (User.Identity.IsAuthenticated)
            {
                DbManager.AddFairyTaleToReadList(fairyTale.Id, User.Identity.GetUserId());
                fairyTale.IsUserLiked = DbManager.IsUserLikedTaleWithId(fairyTale.Id, User.Identity.GetUserId());
                fairyTale.IsUserFavorite = DbManager.IsUserFavoriteTaleWithId(fairyTale.Id, User.Identity.GetUserId());
            }

            return View(fairyTale);
        }

        // POST: Add Tale To Favorites List
        [HttpPost]
        public void FavoriteAction(string path)
        {
            if (string.IsNullOrEmpty(path) || !DbManager.ValidateTaleByPath(path))
                return;

            var fairyTale = DbManager.GetTaleByPath(path);

            if (User.Identity.IsAuthenticated)
                DbManager.AddFairyTaleToFavorites(fairyTale.Id, User.Identity.GetUserId());
        }
    }
}
