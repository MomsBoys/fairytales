using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace FairyTales.Models
{
    public class EmailModel
    {
        [Required]
        [Display(Name = "SenderName")]
        public string SenderName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "SenderEmail")]
        public string SenderEmail { get; set; }
        [Required]
        [Display(Name = "MessageTitle")]
        public string MessageTitle { get; set; }
        [Required]
        [Display(Name = "MessageText")]
        public string MessageText { get; set; }
    }
    public class EmailSender
    {
        private EmailModel _emailModel;
        public EmailSender(EmailModel emailModel)
        {
            _emailModel = emailModel;
        }

        public bool IsEmailSent()
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                // Адреса почти та пароль правдиві, можна на почту глянути за листом)
                client.Credentials = new System.Net.NetworkCredential("fairytales.project.tech@gmail.com", "techsupport");
                MailMessage mm = new MailMessage(_emailModel.SenderEmail, "957267@gmail.com", "Вітання.", "я є)");
                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                client.Send(mm);
            }
            catch (Exception)
            {
                
            }


            return false;
        }
    }
}