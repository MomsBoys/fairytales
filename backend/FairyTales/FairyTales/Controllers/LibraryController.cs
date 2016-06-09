using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FairyTales.Models;
using FairyTales.Models.Pagination;
using Microsoft.AspNet.Identity;

namespace FairyTales.Controllers
{
    public class LibraryController : Controller
    { 
        public ActionResult LastAdded()
        {
            ViewBag.Categories = DbManager.GetCategories();
            ViewBag.Types = DbManager.GetTypes();
            var pagination = new PaginationManager(Filter(FilterMode.LastAdded)) { TalesPerPage = 5 };
            ViewBag.Pagination = pagination;

            int page = 0;
            var value = Request.QueryString["Page"];
            if (!String.IsNullOrWhiteSpace(value))
            {
                page = Int32.Parse(value);
            }
            return View(pagination.GetPage(page));
            //return View(PopulateLikesAndFavorites(DbManager.GetNewShortTales(null, null)));
        }
          
        public ActionResult Popular()
        {
            ViewBag.Categories = DbManager.GetCategories();
            ViewBag.Types = DbManager.GetTypes();
            var pagination = new PaginationManager(Filter(FilterMode.Popular)) { TalesPerPage = 5 };
            ViewBag.Pagination = pagination;

            int page = 0;
            var value = Request.QueryString["Page"];
            if (!String.IsNullOrWhiteSpace(value))
            {
                page = Int32.Parse(value);
            }
            return View(pagination.GetPage(page));
            //return View(PopulateLikesAndFavorites(DbManager.GetPopularShortTales(null, null)));
        }


        private List<FairyTale> Filter(FilterMode mode)
        {
            ViewBag.ToString();
            List<int> categories = null, types = null;
            String value = null;

            value = Request.QueryString["Category"];
            if (!String.IsNullOrWhiteSpace(value))
            {
                categories = value.Split(',').Select(v => Int32.Parse(v)).ToList();

                ViewBag.FilterCategories = categories;
            }

            value = Request.QueryString["Type"];
            if (!String.IsNullOrWhiteSpace(value))
            {
                types = value.Split(',').Select(v => Int32.Parse(v)).ToList();
                ViewBag.FilterTypes = types;
            }

            List<FairyTale> tales;
            switch (mode)
            {
                case FilterMode.Popular:
                    tales = DbManager.GetPopularShortTales(categories, types);
                    break;
                case FilterMode.LastAdded:
                    tales = DbManager.GetNewShortTales(categories, types);
                    break;
                default:
                    tales = DbManager.GetShortTales(categories, types);
                    break;
            }
            return PopulateLikesAndFavorites(tales);
        }

        enum FilterMode { Popular, LastAdded }
         
        private List<FairyTale> PopulateLikesAndFavorites(List<FairyTale> tales)
        {
            if (User.Identity.IsAuthenticated)
                DbManager.PopulateUserLikesAndFavorites(ref tales, User.Identity.GetUserId());

            return tales;
        }

        public ActionResult Recommended()
        {
            if (User.Identity.IsAuthenticated)
                return View(DbManager.GetRecommendedTale(User.Identity.GetUserId()));

            return View();    
        }
    }
}
 