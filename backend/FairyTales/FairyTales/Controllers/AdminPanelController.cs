using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Hosting;
using System.Web.Mvc;
using FairyTales.Entities;
using FairyTales.Models;

namespace FairyTales.Controllers
{
    public class AdminPanelController : Controller
    {
        private readonly AdminPanel _adminPanel;

        public AdminPanelController()
        {
            _adminPanel = new AdminPanel();
        }

        // GET: AdminPanel
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel);
        }
        
        // GET: Tales List
        public ActionResult Tales()
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel.Tales);
        }

        // GET: Add Tale
        public ActionResult AddTale()
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            GetDefaultViewBag();

            return View();
        }
        // Post : Add Tale
        [HttpPost, ValidateInput(false)]
        public ActionResult AddTale(NewTale tale)
        {
            string subPath = "~/Content/Data/" + tale.Name;  
            bool exists = System.IO.Directory.Exists(Server.MapPath(subPath)); 
            if (!exists)
            {
                #region Create root folder
                System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
                subPath = subPath.Remove(0, 2) ;
                FileStream fs1 = new FileStream(HostingEnvironment.ApplicationPhysicalPath + subPath + "/text.txt", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fs1);
                writer.Write(tale.Text);
                writer.Close();
                #endregion
                #region Cover(from url and local)
                if (tale.Cover.Contains("://"))
                {
                    byte[] data;
                    using (WebClient client = new WebClient())
                    {
                        data = client.DownloadData(tale.Cover);
                    }
                    System.IO.File.WriteAllBytes(HostingEnvironment.ApplicationPhysicalPath + subPath + "/img.jpg", data);
                }
                else
                { 
                    System.IO.File.Copy(tale.Cover, HostingEnvironment.ApplicationPhysicalPath + subPath + "/img.jpg"); 
                }

                #endregion
                System.IO.File.Copy(tale.Audio, HostingEnvironment.ApplicationPhysicalPath + subPath + "/audio.mp3");
                var context = new DBFairytaleEntities(); 
                FairyTales.Entities.Tale newTale = new Tale()
                {
                    Audio = "audio.mp3",
                    Author_ID = tale.Author_ID,
                    Category_ID = tale.Category_ID,
                    Cover = "img.jpg",
                    Date = DateTime.Now,
                    LikeCount = 0,
                    Path = tale.Path.Replace(" ", "_"),
                    Text = "text.txt",
                    Type_ID = tale.Type_ID,
                    ShortDescription = tale.ShortDescription,
                    Name = tale.Name,
                    Tale_Tag = new List<Tale_Tag>()
                };
                #region populate tags
                if(tale.Tag != null)
                foreach (var tag in tale.Tag.Split(',').ToArray())
                {
                    tag.Replace(" ", "");
                    if (tag == "") ;
                    else
                    {
                        int iTag;
                        if (Int32.TryParse(tag, out iTag))
                        {
                            if (context.Tags.Where(t => t.Tag_ID == iTag).Count() == 1)
                            {
                                newTale.Tale_Tag.Add(new Tale_Tag() { Tag = context.Tags.Where(t => t.Tag_ID == iTag).First() });
                            }
                        }
                    }
                }
                #endregion

                context.Tales.Add(newTale);
                context.SaveChanges();
                return PartialView("DatabaseUpdated", "New record added.");
            }
                
            else
            {
                return PartialView("DatabaseUpdated", "It looks like we got already this story");
            } 
        }

        // GET: Edit Tale
        public ActionResult EditTale(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            var fairyTale = _adminPanel.Tales.FirstOrDefault(tale => tale.Id == id);

            GetDefaultViewBag();

            return View(fairyTale);
        }

        // GET: Authors List
        public ActionResult Authors()
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel.Authors);
        }

        // GET: Add Author
        public ActionResult AddAuthor()
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            GetDefaultViewBag();

            return View();
        }

        // GET: Edit Author
        public ActionResult EditAuthor(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            var author = _adminPanel.Authors.FirstOrDefault(innerAuthor => innerAuthor.Author_ID == id);

            GetDefaultViewBag();

            return View(author);
        }

        // GET: Categories List
        public ActionResult Categories()
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel.Categories);
        }

        // GET: Add Category
        public ActionResult AddCategory()
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            GetDefaultViewBag();

            return View();
        }

        // GET: Edit Category
        public ActionResult EditCategory(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            var category = _adminPanel.Categories.FirstOrDefault(innerCategory => innerCategory.Category_ID == id);

            GetDefaultViewBag();

            return View(category);
        }

        // GET: Tags List
        public ActionResult Tags()
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel.Tags);
        }

        // GET: Add Tag
        public ActionResult AddTag()
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            GetDefaultViewBag();

            return View();
        }

        // GET: Edit Tag
        public ActionResult EditTag(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            var tag = _adminPanel.Tags.FirstOrDefault(innerTag => innerTag.Tag_ID == id);

            GetDefaultViewBag();

            return View(tag);
        }

        // GET: Users List
        public ActionResult Users()
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel.Users);
        }

        // GET: Edit User
        public ActionResult EditUser(string id)
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            var user = _adminPanel.Users.FirstOrDefault(innerUser => innerUser.Id == id);

            GetDefaultViewBag();

            return View(user);
        }

        #region Additional Methods
        private void GetDefaultViewBag()
        {
            ViewBag.Tales = _adminPanel.Tales;
            ViewBag.Categories = _adminPanel.Categories;
            ViewBag.Tags = _adminPanel.Tags;
            ViewBag.Authors = _adminPanel.Authors;
        }
        #endregion // Additional Methods
    }
}
