using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace FairyTales.Models
{
    public class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
    public class EmailModel
    {
        
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
        private string _email;
        private string _password;
        public EmailSender(EmailModel emailModel,string email,string password)
        {
            _emailModel = emailModel;
            _email = email;
            _password = password;
        }

        public bool IsEmailSent()
        {
            try
            {
                string messageText = string.Format("Привіт мене звати {0}. Мій email: {1}. Моє повідомлення: {2} ", _emailModel.SenderName ?? "Анонім", _emailModel.SenderEmail ,_emailModel.MessageText);
                string messageTitle = string.Format("Питання від користувачів. Тема.{0}", _emailModel.MessageTitle);
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Timeout = 3000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(_email, _password);
                MailMessage mailMessage = new MailMessage(_emailModel.SenderEmail, _email, messageTitle, messageText);
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                client.Send(mailMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }


            
        }
    }
}