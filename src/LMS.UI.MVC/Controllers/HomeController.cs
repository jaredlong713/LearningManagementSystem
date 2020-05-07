using LMS.UI.MVC.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace LMS.UI.MVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Contact(ContactViewModel contact)
        {
            if (ModelState.IsValid)
            {
                string body = $"Message from {contact.Name}<br>" +
                    $"Email: {contact.Email}<br>Message: {contact.Message}";

                MailMessage msg = new MailMessage("no-reply@gmail.com", "Jaredlong713@gmail.com", "Learning Management System", body);

                msg.IsBodyHtml = true;

                using (var mailClient = new SmtpClient())
                {

                    mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mailClient.UseDefaultCredentials = false;
                    mailClient.EnableSsl = true;
                    mailClient.Host = "smtp-mail.outlook.com";
                    mailClient.Port = 587;
                    mailClient.Credentials =
                                    new NetworkCredential(
                                        "jlong@centriq.com",
                                        "Password!");
                    try
                    {
                        await mailClient.SendMailAsync(msg);
                        ViewBag.MailSent =
                            "Email has been sent.";

                        return View(contact);
                    }
                    catch
                    {
                        ViewBag.ErrorMessage = "Sorry, something went wrong.  Please try again or email directly.";

                        return View(contact);
                    }
                }
            }

            return View(contact);
        }
    }
}
