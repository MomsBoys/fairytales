using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FairyTales.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using FairyTales.Models;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FairyTales.Controllers
{

    public class UserSocialController : Controller
    {
        private DBFairytaleEntities db = new DBFairytaleEntities();

        // GET: UserSocial
        public ActionResult Index()
        {
            return View(db.AspNetUsers.ToList());
        }

        public UserSocialController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public UserSocialController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }


        //  UserSocial/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login()
        {
            string link = string.Format("http://ulogin.ru/token.php?token={0}&host={1}", Request.Form["token"],
                Request.ServerVariables["SERVER_NAME"]);

            WebRequest reqGET = WebRequest.Create(link);
            string answer = "";
            using (WebResponse resp = reqGET.GetResponse())
            {
                using (Stream stream = resp.GetResponseStream())
                {
                    if (stream != null)
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            answer = sr.ReadToEnd();
                        }
                }
            }

            JObject obj = JObject.Parse(answer);
            var lastName = obj["last_name"];
            var firstName = obj["first_name"];
            var userID = obj["identity"];
            var social = obj["network"];

            var user = await UserManager.FindAsync(userID.ToString(), social.ToString());
            if (user != null)
            {
                await SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var userAU = new ApplicationUser()
                {
                    UserName = userID.ToString(),
                    FirstName = firstName.ToString(),
                    SecondName = lastName.ToString()
                };
                var result = await UserManager.CreateAsync(userAU, social.ToString());
                if (result.Succeeded)
                {
                    await SignInAsync(userAU, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }          

            return RedirectToAction("Index", "Home");
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        // GET: UserSocial/Details/
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // GET: UserSocial/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserSocial/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserName,PasswordHash,SecurityStamp,FirstName,SecondName,Discriminator")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetUser);
        }

        // GET: UserSocial/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: UserSocial/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,PasswordHash,SecurityStamp,FirstName,SecondName,Discriminator")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }

        // GET: UserSocial/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: UserSocial/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
            db.SaveChanges();
            return RedirectToAction("Index");
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
