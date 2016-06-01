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
      // GET: UserSocial

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
                var cookie = new HttpCookie("first_name", user.FirstName);
                Response.SetCookie(cookie);
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
                    var cookie = new HttpCookie("first_name", userAU.FirstName);
                    Response.SetCookie(cookie);
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

    }
}
