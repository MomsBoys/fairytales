using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FairyTales.Models;

namespace FairyTales.Controllers
{
    public class LibraryController : Controller
    {
        // GET: Library
        public ActionResult Index()
        {
            int i = Request.Form.AllKeys.Count();
            ViewBag.Categories = DbManager.GetCategories();
            ViewBag.Types = DbManager.GetTypes();
            return View(DbManager.GetShortTales(null, null));   
        }
         
        [HttpPost]
        public ActionResult Filter()
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
            return PartialView(DbManager.GetShortTales(categories, types));
        }
    }
}