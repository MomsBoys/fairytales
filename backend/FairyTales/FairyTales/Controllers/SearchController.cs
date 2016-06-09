using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FairyTales.Entities;
using FairyTales.Models;

namespace FairyTales.Controllers
{
    public class SearchController : Controller
    {
        private DBFairytaleEntities db = new DBFairytaleEntities();

        [HttpGet]
        public ActionResult GetListOfTales(string text, string category)
        {
            ViewBag.SeacrhText = text;
            var context = new DBFairytaleEntities();
            switch (category)
            {
                case "tag":
                    ViewBag.Category = "по тегу";
                    ViewBag.Search = DbManager.GetSearchByTag(text);
                    break;

                case "name":
                    ViewBag.Category = " по назві";
                    ViewBag.Search = DbManager.GetSearchByTaleName(text);
                    break;

                case "author":
                    ViewBag.Category = "по автору";
                    ViewBag.Search = DbManager.GetSearchByAuthor(text);
                    break;

                case "all":
                    ViewBag.Category = "по всьому";
                    ViewBag.Search = DbManager.GetSearchByAll(text);
                    break;
            }
            return View("_SearchResult");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
