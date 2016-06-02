using System.Web.Mvc;
using FairyTales.Models;

namespace FairyTales.Controllers
{
    public class FairyTaleController : Controller
    {
        // GET: FairyTale
        public ActionResult Index(string path)
        {
            if (string.IsNullOrEmpty(path))
                return HttpNotFound();

            var fairyTale = DbManager.GetTaleByPath(path);

            if (User.Identity.IsAuthenticated)
            {
                fairyTale.IsUserLiked = DbManager.IsUserLikedTaleWithId(fairyTale.Id, User.Identity.Name);
                fairyTale.IsUserFavorited = DbManager.IsUserFavoritedTaleWithId(fairyTale.Id, User.Identity.Name);
            }

            return View(fairyTale);
        }
    }
}
