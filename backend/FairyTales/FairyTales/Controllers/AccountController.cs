using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using FairyTales.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using FairyTales.Entities;

namespace FairyTales.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    if (user.ConfirmedEmail)
                    {
                        await SignInAsync(user, model.RememberMe);

                        var cookie = new HttpCookie("first_name", user.FirstName);
                        Response.SetCookie(cookie);
                        return JavaScript("location.reload(true)");
                    }
                    else
                    {
                        ViewBag.ErrorSignIn = "Підтвердіть свій email.";
                        return PartialView("_SignInPartial", model);
                    }
                }
                else
                {
                    ViewBag.ErrorSignIn = "Невірний email або пароль.";
                    return PartialView("_SignInPartial", model);
                }
            }
            ViewBag.ErrorSignIn = "Некоректний email або пароль.";
            return PartialView("_SignInPartial", model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            string errorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    SecondName = model.SecondName,
                    Email = model.UserName,
                    ConfirmedEmail = false
                };
                
                var result = await UserManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    var configuration = System.Web.Configuration.WebConfigurationManager.AppSettings;
                    string message = string.Format("Для завершення реєстрації перейдіть за посиланням:" +
                                    "<a href=\"{0}\" title=\"Підтвердити реєстрацію\">{0}</a>",
                        Url.Action("ConfirmEmail", "Account", new { Token = user.Id, Email = user.Email }, Request.Url.Scheme));
                    var emailSender = new ConfirmAccountEmailSender(configuration["Email"], configuration["EmailPass"], user.Email);
                    if (emailSender.IsEmailSent(message, "Завершення реєстрації"))
                    {
                        return PartialView("_SignUpSuccessfulPartial", "На Вашу електронну адресу відправлено лист для завершення реєстрації.");
                    }
                    else
                    {
                        ViewBag.ErrorSignUp = "Вибачте реєстрація пройшла не успішно.";
                        return PartialView("_SignUpPartial", model);
                    }

                }
                ViewBag.ErrorSignUp = "Користувач з введеним email вже існує. Введіть інший email.";
                return PartialView("_SignUpPartial", model);
            }
            ViewBag.ErrorSignUp = "Пароль повинен містити мінімум 6 символів.";
            return PartialView("_SignUpPartial", model);
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string Token, string Email)
        {
            ApplicationUser user = this.UserManager.FindById(Token);
            if (user != null)
            {
                if (user.Email == Email)
                {
                    user.ConfirmedEmail = true;
                    await UserManager.UpdateAsync(user);
                    await SignInAsync(user, isPersistent: false);
                    var cookie = new HttpCookie("first_name", user.FirstName);
                    Response.SetCookie(cookie);
                    return RedirectToAction("LastAdded", "Library");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: /Account/ForgetPass
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgetPass()
        {
            return PartialView("_SignInForgotPass",new ForgotPassViewModel());
        }

        // POST: /Account/LoginBack
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginBack()
        {
            return PartialView("_SignInPartial", new LoginViewModel());
        }

        // POST: /Account/ForgotPassEmail
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassEmail(ForgotPassViewModel model)
        {
            if (ModelState.IsValid)
            {
                AspNetUser user = null;
                using (var context = new DBFairytaleEntities())
                {
                    var query = context.AspNetUsers.Where(e => e.Email == model.Email);
                    if (query.Any())
                    {
                        user = query.First();
                    }
                }
                if (user!=null)
                {
                    var configuration = System.Web.Configuration.WebConfigurationManager.AppSettings;
                    string message = string.Format("Для відновлення паролю перейдіть за посиланням:" +
                                    "<a href=\"{0}\" title=\"Підтвердити реєстрацію\">{0}</a>",
                        Url.Action("PasswordRecovery", "Account", new { Token = user.Id }, Request.Url.Scheme));
                    var emailSender = new ConfirmAccountEmailSender(configuration["Email"], configuration["EmailPass"], user.Email);
                    if (emailSender.IsEmailSent(message, "Відновлення паролю"))
                    {
                        return PartialView("_SignUpSuccessfulPartial", "На Вашу електронну адресу відправлено лист для відновлення паролю.");
                    }
                    else
                    {
                        ViewBag.ErrorSignInForgotPass = "Вибачте відновлення пройшло не успішно. Спробуйте згодом.";
                        return PartialView("_SignInForgotPass", model);
                    }
                }
                else
                {
                    ViewBag.ErrorSignInForgotPass = "Користувача з введеним email не знайдено.";
                    return PartialView("_SignInForgotPass", model);
                }
            }
            else
            {
                ViewBag.ErrorSignInForgotPass = "Некоректно введений email.";
                return PartialView("_SignInForgotPass", model);
            }
            
        }

        // GET: /Account/PasswordRecovery
        [AllowAnonymous]
        public ActionResult PasswordRecovery(string token)
        {
            ViewBag.RecoveryPassToken = token;
            return View();
        }

        // POST: /Account/PasswordRecovery
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PasswordRecovery(string email, string password, string token)
        {
            if (email.IsEmpty())
                return PartialView("_ErrorPartial", "Введіть email.");
            if (password.Length < 6 || password.IsEmpty())
                return PartialView("_ErrorPartial", "Пароль повинен містити мінімум 6 символів");
            ApplicationUser user = this.UserManager.FindById(token);
            if (user != null)
            {
                if(user.Email != email)
                    return PartialView("_ErrorPartial", "Неправильний email.");
                await UserManager.RemovePasswordAsync(user.Id);
                await UserManager.AddPasswordAsync(user.Id, password);
                await SignInAsync(user, isPersistent: false);
                var cookie = new HttpCookie("first_name", user.FirstName);
                Response.SetCookie(cookie);
                return JavaScript("window.location = '/library/lastadded'");
            }
            return JavaScript("window.location = '/'");
        }


        public ActionResult Redirect()
        {
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            if (Request.Cookies["first_name"] != null)
            {
                HttpCookie cookie = new HttpCookie("first_name");
                cookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

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

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}