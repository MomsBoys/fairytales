using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.IO;
using FairyTales.Models;
using Newtonsoft.Json.Linq;

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
            var link = string.Format("http://ulogin.ru/token.php?token={0}&host={1}", Request.Form["token"],
                Request.ServerVariables["SERVER_NAME"]);

            var reqGet = WebRequest.Create(link);
            var answer = "";
            using (var resp = reqGet.GetResponse())
            {
                using (var stream = resp.GetResponseStream())
                {
                    if (stream != null)
                        using (var sr = new StreamReader(stream))
                        {
                            answer = sr.ReadToEnd();
                        }
                }
            }

            var obj = JObject.Parse(answer);
            var lastName = obj["last_name"];
            var firstName = obj["first_name"];
            var userId = obj["identity"];
            var social = obj["network"];

            var user = await UserManager.FindAsync(userId.ToString(), social.ToString());
            if (user != null)
            {
                await SignInAsync(user, false);
                var cookie = new HttpCookie("first_name", user.FirstName);
                Response.SetCookie(cookie);
                return RedirectToAction("LastAdded", "Library");
            }

            var appUser = new ApplicationUser()
            {
                UserName = userId.ToString(),
                FirstName = firstName.ToString(),
                SecondName = lastName.ToString()
            };

            var result = await UserManager.CreateAsync(appUser, social.ToString());
            if (result.Succeeded)
            {
                await SignInAsync(appUser, isPersistent: false);
                var cookie = new HttpCookie("first_name", appUser.FirstName);
                Response.SetCookie(cookie);
                return RedirectToAction("LastAdded", "Library");
            }

            AddErrors(result);

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
