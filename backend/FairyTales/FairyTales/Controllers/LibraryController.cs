using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FairyTales.Models;

namespace FairyTales.Controllers
{
    public class LibraryController : Controller
    {
        // GET: Library
        public ActionResult LastAdded()
        {
            ViewBag.Categories = DBManager.GetCategories();
            ViewBag.Types = DBManager.GetTypes();
            return View(DBManager.GetNewShortTales(null, null));   
        }
         
        public ActionResult Popular()
        {
            ViewBag.Categories = DBManager.GetCategories();
            ViewBag.Types = DBManager.GetTypes();
            return View(DBManager.GetPopularShortTales(null, null));   
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

            if (Request.Form["type"] == "popular")
            {
                return PartialView(DBManager.GetPopularShortTales(categories, types));
            }
            if (Request.Form["type"] == "lastadded")
            {
                return PartialView(DBManager.GetNewShortTales(categories, types));
            }

            return PartialView(DBManager.GetShortTales(categories, types));
        }
    }
}