using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.WebPages;
using FairyTales.Entities;
using FairyTales.Models;
using Microsoft.AspNet.Identity;
using Type = FairyTales.Entities.Type;

namespace FairyTales.Controllers
{
    public class AdminPanelController : Controller
    {
        private readonly AdminPanel _adminPanel;

        public AdminPanelController()
        {
            _adminPanel = new AdminPanel();
        }

        private bool IsAdminUser()
        {
            return User.Identity.IsAuthenticated && DbManager.CurrentUser(User.Identity.GetUserId()).IsAdmin;
        }

        // GET: AdminPanel
        public ActionResult Index()
        {
            if (!IsAdminUser())
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel);
        }

        #region Tales 
        // GET: Tales List
        public ActionResult Tales()
        {
            if (!IsAdminUser())
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel.Tales);
        }

        // GET: Add Tale
        public ActionResult AddTale()
        {
            if (!IsAdminUser())
                return PartialView("Error");

            GetDefaultViewBag();
            ViewBag.Types = _adminPanel.Types;

            return View(new FairyTale());
        }

        // POST: Add Tale
        [HttpPost, ValidateInput(false)]
        public ActionResult AddTale(FairyTale tale)
        {
            if (!IsAdminUser() || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();
            ViewBag.Types = _adminPanel.Types;

            if (TaleHasEmptyFields(tale))
            {
                ViewBag.ResponseResult = ResponseType.EmptyValues;
                return View("AddTale", tale);
            }

            tale.Name = tale.Name.Trim();
            var talePath = string.Format("~/Content/Data/{0}", tale.Name);
            var isFolderExists = Directory.Exists(Server.MapPath(talePath));

            if (isFolderExists)
            {
                ViewBag.ResponseResult = ResponseType.Exists;
                return View("AddTale", tale);
            }

            // Create root folder
            Directory.CreateDirectory(Server.MapPath(talePath));
            talePath = talePath.Remove(0, 2);
            var fileStream = new FileStream(HostingEnvironment.ApplicationPhysicalPath + talePath + "/text.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            writer.Write(tale.TextPath);
            writer.Close();

            // Create cover (from url or local)
            if (tale.CoverPath.Contains("://"))
            {
                byte[] data;
                using (var client = new WebClient())
                {
                    data = client.DownloadData(tale.CoverPath);
                }
                System.IO.File.WriteAllBytes(HostingEnvironment.ApplicationPhysicalPath + talePath + "/img.jpg", data);
            }
            else
            {
                System.IO.File.Copy(tale.CoverPath, HostingEnvironment.ApplicationPhysicalPath + talePath + "/img.jpg");
            }

            // Add Audio
            if (tale.AudioPath != null)
            {
                if (tale.AudioPath.Contains("://"))
                {
                    byte[] data;
                    using (var client = new WebClient())
                    {
                        data = client.DownloadData(tale.AudioPath);
                    }

                    System.IO.File.WriteAllBytes(HostingEnvironment.ApplicationPhysicalPath + talePath + "/audio.mp3",
                        data);
                }
                else
                {
                    System.IO.File.Copy(tale.AudioPath,
                        HostingEnvironment.ApplicationPhysicalPath + talePath + "/audio.mp3");
                }
            }

            var operationResult = DbManager.AddNewTale(tale);
            ViewBag.ResponseResult = operationResult;

            if (operationResult == ResponseType.Error || operationResult == ResponseType.Exists)
                return View("AddTale", tale);

            ModelState.Clear();
            return View("AddTale", new FairyTale());
        }

        private static bool TaleHasEmptyFields(FairyTale tale)
        {
            return tale.Name.IsEmpty() ||
                tale.Path.IsEmpty() ||
                tale.TextPath.IsEmpty() ||
                tale.ShortDescription.IsEmpty() ||
                tale.CoverPath.IsEmpty() ||
                tale.AuthorInput.IsEmpty() ||
                tale.CategoryInput.IsEmpty() ||
                tale.TypeInput.IsEmpty();
        }

        // GET: Edit Tale
        public ActionResult EditTale(int id)
        {
            if (!IsAdminUser())
                return PartialView("Error");

            var fairyTale = _adminPanel.Tales.FirstOrDefault(tale => tale.Id == id);

            if (fairyTale == null)
                return PartialView("Error");

            fairyTale.AuthorInput = string.Format("{0} {1}", fairyTale.Author.LastName, fairyTale.Author.FirstName).Trim();
            fairyTale.CategoryInput = fairyTale.Category.Name;
            fairyTale.TypeInput = fairyTale.Type.Name;
            fairyTale.TextPath = fairyTale.Text;

            var allTags = _adminPanel.Tags;
            var selectedTagsNames = new List<string>();

            foreach (var selectedTag in fairyTale.Tags)
            {
                selectedTagsNames.AddRange(from tag in allTags where tag.Tag_ID == selectedTag.Tag_ID select selectedTag.Name);
            }

            fairyTale.SelectedTags = selectedTagsNames.ToArray();

            GetDefaultViewBag();
            ViewBag.Types = _adminPanel.Types;

            return View(fairyTale);
        }

        // POST: Add Tale
        [HttpPost, ValidateInput(false)]
        public ActionResult EditTale(FairyTale tale)
        {
            if (!IsAdminUser() || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();
            ViewBag.Types = _adminPanel.Types;

            if (TaleHasEmptyFields(tale))
            {
                ViewBag.ResponseResult = ResponseType.EmptyValues;
                return View("EditTale", tale);
            }

            var talePath = string.Format("~/Content/Data/{0}", tale.Name);
            var isFolderExists = Directory.Exists(Server.MapPath(talePath));

            if (!isFolderExists)
            {
                ViewBag.ResponseResult = ResponseType.Error;
                return RedirectToAction("Tales");
            }

            // Use existing folder
            talePath = talePath.Remove(0, 2);
            var fileStream = new FileStream(HostingEnvironment.ApplicationPhysicalPath + talePath + "/text.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            writer.Write(tale.TextPath);
            writer.Close();

            // Update cover (from url or local)
            if (tale.CoverPath.Contains("://"))
            {
                byte[] data;
                using (var client = new WebClient())
                {
                    data = client.DownloadData(tale.CoverPath);
                }

                System.IO.File.WriteAllBytes(HostingEnvironment.ApplicationPhysicalPath + talePath + "/img.jpg",
                    data);
            }
            else
            {
                System.IO.File.Copy(tale.CoverPath,
                        HostingEnvironment.ApplicationPhysicalPath + talePath + "/img.jpg");
            }

            // Update Audio
            if (tale.AudioPath != null)
            {
                if (tale.AudioPath.Contains("://"))
                {
                    byte[] data;
                    using (var client = new WebClient())
                    {
                        data = client.DownloadData(tale.AudioPath);
                    }

                    System.IO.File.WriteAllBytes(HostingEnvironment.ApplicationPhysicalPath + talePath + "/audio.mp3",
                        data);
                }
                else
                {
                    System.IO.File.Copy(tale.AudioPath,
                        HostingEnvironment.ApplicationPhysicalPath + talePath + "/audio.mp3");
                }
            }

            var operationResult = DbManager.EditExistingTale(tale);
            ViewBag.ResponseResult = operationResult;

            if (operationResult == ResponseType.Error || operationResult == ResponseType.Exists)
                return View("AddTale", tale);

            ModelState.Clear();
            return View("AddTale", new FairyTale());
        }
        #endregion // Tales

        #region Authors
        // GET: Authors List
        public ActionResult Authors()
        {
            if (!IsAdminUser())
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel.Authors);
        }

        // GET: Add Author
        public ActionResult AddAuthor()
        {
            if (!IsAdminUser())
                return PartialView("Error");

            GetDefaultViewBag();

            return View(new Author());
        }

        // POST: Add Author
        [HttpPost]
        public ActionResult AddAuthor(Author author)
        {
            if (!IsAdminUser() || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();

            var operationResult = author.FirstName.IsEmpty() || author.LastName.IsEmpty() ? ResponseType.EmptyValues : DbManager.AddNewAuthor(author);
            ViewBag.ResponseResult = operationResult;

            if (operationResult == ResponseType.EmptyValues ||
                operationResult == ResponseType.Exists)
                return View("AddAuthor", author);

            ModelState.Clear();
            return View("AddAuthor", new Author());
        }

        // GET: Edit Author
        public ActionResult EditAuthor(int id)
        {
            if (!IsAdminUser())
                return PartialView("Error");

            var author = _adminPanel.Authors.FirstOrDefault(innerAuthor => innerAuthor.Author_ID == id);

            GetDefaultViewBag();

            return View(author);
        }

        // POST: Edit Author
        [HttpPost]
        public ActionResult EditAuthor(Author author)
        {
            if (!IsAdminUser() || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();

            author.FirstName = Request.Form["first_name"];
            author.LastName = Request.Form["last_name"];

            var operationResult = author.FirstName.IsEmpty() || author.LastName.IsEmpty() ? ResponseType.EmptyValues : DbManager.EditExistingAuthor(author);
            ViewBag.ResponseResult = operationResult;

            if (operationResult == ResponseType.EmptyValues || operationResult == ResponseType.Error)
                return View("EditAuthor", author);

            ModelState.Clear();

            return View("AddAuthor", new Author());
        }
        #endregion // Authors

        #region Categories
        // GET: Categories List
        public ActionResult Categories()
        {
            if (!IsAdminUser())
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel.Categories);
        }

        // GET: Add Category
        public ActionResult AddCategory()
        {
            if (!IsAdminUser())
                return PartialView("Error");

            GetDefaultViewBag();

            return View(new Category());
        }

        // POST: Add Category
        [HttpPost]
        public ActionResult AddCategory(Category category)
        {
            if (!IsAdminUser() || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();

            var operationResult = category.Name.IsEmpty() ? ResponseType.EmptyValues : DbManager.AddNewCategory(category);
            ViewBag.ResponseResult = operationResult;

            if (operationResult == ResponseType.EmptyValues ||
                operationResult == ResponseType.Exists)
                return View("AddCategory", category);

            ModelState.Clear();
            return View("AddCategory", new Category());
        }

        // GET: Edit Category
        public ActionResult EditCategory(int id)
        {
            if (!IsAdminUser())
                return PartialView("Error");

            var category = _adminPanel.Categories.FirstOrDefault(innerCategory => innerCategory.Category_ID == id);

            if (category == null)
                return PartialView("Error");

            GetDefaultViewBag();

            return View(category);
        }

        // POST: Edit Category
        [HttpPost]
        public ActionResult EditCategory(Category category)
        {
            if (!IsAdminUser() || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();

            category.Category_ID = Convert.ToInt32(Request.Form["category_id"]);
            category.Name = Request.Form["category_name"];

            var operationResult = category.Name.IsEmpty() ? ResponseType.EmptyValues : DbManager.EditExistingCategory(category);
            ViewBag.ResponseResult = operationResult;

            if (operationResult == ResponseType.EmptyValues || operationResult == ResponseType.Error)
                return View("EditCategory", category);

            ModelState.Clear();
            return View("AddCategory", new Category());
        }
        #endregion // Categories

        #region Types
        // GET: Types List
        public ActionResult Types()
        {
            if (!IsAdminUser())
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel.Types);
        }

        // GET: Add Type
        public ActionResult AddType()
        {
            if (!IsAdminUser())
                return PartialView("Error");

            GetDefaultViewBag();

            return View(new Type());
        }

        // POST: Add Type
        [HttpPost]
        public ActionResult AddType(Type type)
        {
            if (!IsAdminUser() || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();

            var operationResult = type.Name.IsEmpty() ? ResponseType.EmptyValues : DbManager.AddNewType(type);
            ViewBag.ResponseResult = operationResult;

            if (operationResult == ResponseType.EmptyValues ||
                operationResult == ResponseType.Exists)
                return View("AddType", type);

            ModelState.Clear();
            return View("AddType", new Type());
        }

        // GET: Edit Type
        public ActionResult EditType(int id)
        {
            if (!IsAdminUser())
                return PartialView("Error");

            var type = _adminPanel.Types.FirstOrDefault(innerType => innerType.Type_ID == id);

            GetDefaultViewBag();

            return View(type);
        }

        // POST: Edit Type
        [HttpPost]
        public ActionResult EditType(Type type)
        {
            if (!IsAdminUser() || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();

            type.Type_ID = Convert.ToInt32(Request.Form["type_id"]);
            type.Name = Request.Form["type_name"];

            var operationResult = type.Name.IsEmpty() ? ResponseType.EmptyValues : DbManager.EditExistingType(type);
            ViewBag.ResponseResult = operationResult;

            if (operationResult == ResponseType.EmptyValues || operationResult == ResponseType.Error)
                return View("EditType", type);

            ModelState.Clear();
            return View("AddType", new Type());
        }
        #endregion // Types

        #region Tags
        // GET: Tags List
        public ActionResult Tags()
        {
            if (!IsAdminUser())
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel.Tags);
        }

        // GET: Add Tag
        public ActionResult AddTag()
        {
            if (!IsAdminUser())
                return PartialView("Error");

            GetDefaultViewBag();

            return View(new Tag());
        }

        // POST: Add Tag
        [HttpPost]
        public ActionResult AddTag(Tag tag)
        {
            if (!IsAdminUser() || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();

            var operationResult = tag.Name.IsEmpty() ? ResponseType.EmptyValues : DbManager.AddNewTag(tag);
            ViewBag.ResponseResult = operationResult;

            if (operationResult == ResponseType.EmptyValues ||
                operationResult == ResponseType.Exists)
                return View("AddTag", tag);

            ModelState.Clear();
            return View("AddTag", new Tag());
        }

        // GET: Edit Tag
        public ActionResult EditTag(int id)
        {
            if (!IsAdminUser())
                return PartialView("Error");

            var tag = _adminPanel.Tags.FirstOrDefault(innerTag => innerTag.Tag_ID == id);

            GetDefaultViewBag();

            return View(tag);
        }

        // POST: Edit Tag
        [HttpPost]
        public ActionResult EditTag(Tag tag)
        {
            if (!IsAdminUser() || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();

            tag.Tag_ID = Convert.ToInt32(Request.Form["tag_id"]);
            tag.Name = Request.Form["tag_name"];

            var operationResult = tag.Name.IsEmpty() ? ResponseType.EmptyValues : DbManager.EditExistingTag(tag);
            ViewBag.ResponseResult = operationResult;

            if (operationResult == ResponseType.EmptyValues || operationResult == ResponseType.Error)
                return View("EditTag", tag);

            ModelState.Clear();
            return View("AddTag", new Tag());
        }
        #endregion // Tags

        #region Users
        // GET: Users List
        public ActionResult Users()
        {
            if (!IsAdminUser())
                return PartialView("Error");

            GetDefaultViewBag();

            return View(_adminPanel.Users);
        }

        // GET: Edit User
        public ActionResult EditUser(string id)
        {
            if (!IsAdminUser())
                return PartialView("Error");

            var user = _adminPanel.Users.FirstOrDefault(innerUser => innerUser.Id == id);

            if (user == null)
                return PartialView("Error");

            GetDefaultViewBag();

            return View(user);
        }

        // POST: Edit User
        [HttpPost]
        public ActionResult EditUser(AspNetUser user)
        {
            if (!IsAdminUser() || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();

            user.FirstName = Request.Form["first_name"];
            user.SecondName = Request.Form["last_name"];

            if (Request.Form.AllKeys.Contains("isadmin"))
                user.IsAdmin = true;

            var hasEmptyFields = user.FirstName.IsEmpty() || user.SecondName.IsEmpty();
            var operationResult = hasEmptyFields ? ResponseType.EmptyValues : DbManager.EditExistingUser(user);
            ViewBag.ResponseResult = operationResult;

            if (operationResult == ResponseType.EmptyValues || operationResult == ResponseType.Error)
                return View("EditUser", user);

            ModelState.Clear();
            return RedirectToAction("Users");
        }
        #endregion // Users

        #region Additional Methods
        private void GetDefaultViewBag()
        {
            ViewBag.Tales = _adminPanel.Tales;
            ViewBag.Categories = _adminPanel.Categories;
            ViewBag.Tags = _adminPanel.Tags;
            ViewBag.Authors = _adminPanel.Authors;
            ViewBag.Types = _adminPanel.Types;
        }
        #endregion // Additional Methods
    }
}
