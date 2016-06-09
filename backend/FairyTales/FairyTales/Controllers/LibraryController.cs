using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FairyTales.Models;
using Microsoft.AspNet.Identity;

namespace FairyTales.Controllers
{
    public class LibraryController : Controller
    {
        // GET: Library
        public ActionResult LastAdded()
        {
            ViewBag.Categories = DbManager.GetCategories();
            ViewBag.Types = DbManager.GetTypes();
            return View(PopulateLikesAndFavorites(DbManager.GetNewShortTales(null, null)));   
        }

         
        public ActionResult Popular()
        {
            ViewBag.Categories = DbManager.GetCategories();
            ViewBag.Types = DbManager.GetTypes();
            return View(PopulateLikesAndFavorites(DbManager.GetPopularShortTales(null, null)));   
        }


        public ActionResult Favourite()
        {
            @ViewBag.ros = "kuku";
            ViewBag.Categories = DbManager.GetCategories();
            ViewBag.Types = DbManager.GetTypes();
            if (User.Identity.IsAuthenticated)
            {
                return View(PopulateLikesAndFavorites(DbManager.GetFavouriteShortTales(User.Identity.GetUserId())));
            }
            
            return View(PopulateLikesAndFavorites(DbManager.GetNewShortTales(null, null)));
        }

        public ActionResult RecentReaded()
        {
            ViewBag.Categories = DbManager.GetCategories();
            ViewBag.Types = DbManager.GetTypes();
            if (User.Identity.IsAuthenticated)
            {
                return View(PopulateLikesAndFavorites(DbManager.GetRecentReadedShortTales(User.Identity.GetUserId())));
            }

            return View(PopulateLikesAndFavorites(DbManager.GetNewShortTales(null, null)));
        }

        [HttpPost]
        public ActionResult Filter(int? id)
        {
            List<int> categories = null, types = null;

            if (Request.Form.AllKeys.Count() != 0)
            {
                categories = new List<int>();
                types = new List<int>();
                foreach (var key in Request.Form.AllKeys)
                {
                    if (key.Contains("cKey"))
                    { 
                        categories.Add(int.Parse(key.Replace("cKey", "")));
                    }
                    if (key.Contains("tKey"))
                    {
                        types.Add(int.Parse(key.Replace("tKey", "")));
                    }
                }
            }

            List<FairyTale> tales;

            if (Request.Form["type"] == "popular")
                tales = DbManager.GetPopularShortTales(categories, types);
            else if (Request.Form["type"] == "lastadded")
                tales = DbManager.GetNewShortTales(categories, types);
            else
                tales = DbManager.GetShortTales(categories, types);

            return PartialView(PopulateLikesAndFavorites(tales));
        }

        private List<FairyTale> PopulateLikesAndFavorites(List<FairyTale> tales)
        {
            if (User.Identity.IsAuthenticated)
                DbManager.PopulateUserLikesAndFavorites(ref tales, User.Identity.GetUserId());

            return tales;
        }
    }
}