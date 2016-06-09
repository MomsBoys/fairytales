using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
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

            return View(new Author());
        }

        // POST: Add Author
        [HttpPost]
        public ActionResult AddAuthor(Author author)
        {
            if (!User.Identity.IsAuthenticated || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();

            author.FirstName = Request.Form["first_name"];
            author.LastName = Request.Form["last_name"];

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
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            var author = _adminPanel.Authors.FirstOrDefault(innerAuthor => innerAuthor.Author_ID == id);

            GetDefaultViewBag();

            return View(author);
        }

        // POST: Edit Author
        [HttpPost]
        public ActionResult EditAuthor(Author author)
        {
            if (!User.Identity.IsAuthenticated || !Request.Form.AllKeys.Any())
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
            
            return View(new Category());
        }

        // POST: Add Category
        [HttpPost]
        public ActionResult AddCategory(Category category)
        {
            if (!User.Identity.IsAuthenticated || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();

            category.Name = Request.Form["category_name"];

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
            if (!User.Identity.IsAuthenticated)
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
            if (!User.Identity.IsAuthenticated || !Request.Form.AllKeys.Any())
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

            return View(new Tag());
        }

        // POST: Add Tag
        [HttpPost]
        public ActionResult AddTag(Tag tag)
        {
            if (!User.Identity.IsAuthenticated || !Request.Form.AllKeys.Any())
                return PartialView("Error");

            GetDefaultViewBag();

            tag.Name = Request.Form["tag_name"];

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
            if (!User.Identity.IsAuthenticated)
                return PartialView("Error");

            var tag = _adminPanel.Tags.FirstOrDefault(innerTag => innerTag.Tag_ID == id);

            GetDefaultViewBag();

            return View(tag);
        }

        // POST: Edit Tag
        [HttpPost]
        public ActionResult EditTag(Tag tag)
        {
            if (!User.Identity.IsAuthenticated || !Request.Form.AllKeys.Any())
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

            if (user == null)
                return PartialView("Error");

            GetDefaultViewBag();

            return View(user);
        }

        // POST: Edit User
        [HttpPost]
        public ActionResult EditUser(AspNetUser user)
        {
            if (!User.Identity.IsAuthenticated || !Request.Form.AllKeys.Any())
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
