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
            ViewBag.Categories = DbManager.GetCategories();
            ViewBag.Types = DbManager.GetTypes();
            return View(DbManager.GetNewShortTales(null, null));   
        }
         
        public ActionResult Popular()
        {
            ViewBag.Categories = DbManager.GetCategories();
            ViewBag.Types = DbManager.GetTypes();
            return View(DbManager.GetPopularShortTales(null, null));   
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
                return PartialView(DbManager.GetPopularShortTales(categories, types));
            }
            if (Request.Form["type"] == "lastadded")
            {
                return PartialView(DbManager.GetNewShortTales(categories, types));
            }

            return PartialView(DbManager.GetShortTales(categories, types));
        }
    }
}