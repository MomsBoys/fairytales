using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FairyTales.Models;
using Newtonsoft.Json;

namespace FairyTales.Controllers
{

    public class ContactController : Controller
    {
        // GET: /Contact/
        public ActionResult Index()
        {
            ViewBag.hello = "sdsdsd";
            return View(new EmailModel());
        }

        // POST: /Contact/Sender
        [HttpPost]
        [AllowAnonymous]
       public async Task<ActionResult> Sender(EmailModel emailModel)
        {
            string errorMessage;
            var configuration = System.Web.Configuration.WebConfigurationManager.AppSettings;
            var response = Request["g-recaptcha-response"];
            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", configuration["ReCaptchaPrivateKey"], response));

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);
            if (!captchaResponse.Success)
            {
                errorMessage = "Ви не пройшли перевірку reCAPTCHA";
                ViewBag.ErrorMessage = errorMessage;
            }
            else
            {
                EmailSender emailSender = new EmailSender(emailModel, configuration["Email"], configuration["EmailPass"]);
                if (ModelState.IsValid)
                {
                    if (emailSender.IsEmailSent())
                    {
                        return PartialView("_SendSuccess");
                    }
                    else
                    {
                        errorMessage = "Вибачте, неможливо відправити повідомлення.";
                        ViewBag.ErrorMessage = errorMessage;
                    }

                }
                else
                {
                    errorMessage = "Неправильно заповненені поля.";
                    ViewBag.ErrorMessage = errorMessage;
                    
                }
             }
            return PartialView("_FormContact", emailModel);
        }
	}
}