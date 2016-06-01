using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using FairyTales.Entities;
using FairyTales.Models;

namespace FairyTales.Controllers
{
    public class LibraryController : Controller
    {
        // GET: Library
        public ActionResult Index()
        { 
            return View(DBManager.GetShortTales(null, null));   
        }
    }
}