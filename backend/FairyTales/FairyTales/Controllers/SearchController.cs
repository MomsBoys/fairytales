using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FairyTales.Entities;
using FairyTales.Models;
using Microsoft.AspNet.Identity;

namespace FairyTales.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/
        [HttpGet]
        public ActionResult Index(string text, string category)
        {
            ViewBag.SeacrhText = text;
            List<FairyTale> resultSearch;
            switch (category)
            {
                case "tag":
                    ViewBag.Category = "по тегу";
                    resultSearch = PopulateLikesAndFavorites(DbManager.GetSearchByTag(text));
                    break;

                case "name":
                    ViewBag.Category = "по назві";
                    resultSearch = PopulateLikesAndFavorites(DbManager.GetSearchByTaleName(text));
                    break;

                case "author":
                    ViewBag.Category = "по автору";
                    resultSearch = PopulateLikesAndFavorites(DbManager.GetSearchByAuthor(text));
                    break;

                case "all":
                    ViewBag.Category = "по всьому";
                    resultSearch = PopulateLikesAndFavorites(DbManager.GetSearchByAll(text));
                    break;
                default:
                    resultSearch = new List<FairyTale>();
                    break;
            }
            return View(resultSearch);
        }

        private List<FairyTale> PopulateLikesAndFavorites(List<FairyTale> tales)
        {
            if (User.Identity.IsAuthenticated)
                DbManager.PopulateUserLikesAndFavorites(ref tales, User.Identity.GetUserId());

            return tales;
        }
	}
}