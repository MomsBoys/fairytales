using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FairyTales.Models;

namespace FairyTales.Controllers
{
    public class ContactController : Controller
    {
        //
        // GET: /Contact/
        public ActionResult Index()
        {
            return View();
        }

        // POST: /Contact/Sender
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Sender(EmailModel emailModel)
        {
            EmailSender emailSender = new EmailSender(emailModel);
            if (emailSender.IsEmailSent())
            {
                RedirectToAction("Index", "Contact");
            }


            return RedirectToAction("Index", "Home");
        }
	}
}