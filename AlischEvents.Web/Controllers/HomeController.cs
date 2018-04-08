using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlischEvents.Web.Models;
using System.Text;
using AlischEvents.Web.Models.Admin;
using AlischEvents.Web.Helpers;
using log4net;

namespace AlischEvents.Web.Controllers
{
    public class HomeController : Controller
    {
        ILog logger = LogManager.GetLogger(typeof(HomeController));

        AlischDB db = new AlischDB();

        //
        // GET: /Home/

        public ActionResult Index()
        {
            
            return View(db.WebSites.FirstOrDefault(s => s.SiteID == 1));
        }

        //
        // GET: /Home/Kontakt/

        public ActionResult Kontakt() {

            return View(db.WebSites.FirstOrDefault(s => s.SiteID == 2));
        }

        //
        // POST: /Home/Kontakt/
        [HttpPost]
        public ActionResult Kontakt(FormCollection items) {

            //Wenn eins der Felder nicht ausgefüllt wurde Nachricht nicht verarbeiten
            bool cancel = false;
            foreach (string item in items) {
                if (items[item] == null || items[item] == "") {
                    cancel = true;
                    logger.Debug("Not all elements were filled out! Can not submit contact form!");   
                }
            }
            if (cancel) {
                ViewBag.ErrorMessage = "Bitte füllen Sie alle Felder aus.";
                return View(db.WebSites.FirstOrDefault(s => s.SiteID == 2));
            }

            string email = "";
            string subject = "kein Betreff angegeben";
            StringBuilder content = new StringBuilder();

            content.AppendLine(@"<h3>Folgende Nachricht wurde an das Kontaktsystem übermittelt:</h3>");
            content.AppendLine(@"<table border='0' cellpadding='5'>");

            foreach (string item in items) {
                if (!item.Equals("submit")) {
                    string text = items[item];
                    if (items[item].Contains("\n")) {
                        //Wenn Umbrüche vorhanden sind, müssen diese auf HTML gemappt werden
                        text = items[item].Replace("\n", "<br />");
                    }
                    content.AppendLine(@"<tr><td><strong>").Append(item).Append(@"</strong></td><td>").Append(text).Append(@"</td></tr>");

                    //E-Mailadressen erkennen
                    if(item.ToLower().Equals("email")){
                        email = items[item];
                        logger.Debug("Found sender field in contact form!");
                    }

                    //Betreff erkennen
                    if (item.ToLower().Equals("betreff")) {
                        subject = items[item];
                        logger.Debug("Found subject field in contact form!");
                    }
                }
            }

            content.AppendLine(@"</table>");

            content.AppendLine(@"<p>Ankunft der Nachricht: ").Append(DateTime.Now).Append(@"</p>");
            if (!email.Equals("")) {
                content.AppendLine(@"<p>Klicken Sie hier um auf diese Nachricht zu <a href='mailto:").Append(email).Append(@"' target='_blank'>Antworten</a></p>");
            } else {
                content.AppendLine(@"<p>Es konnte kein Feld mit einer Antwortadresse ermittelt werden. Bitte ggf. die Adresse manuell aus der Nachricht lesen!</p>");
            }

            db.ContactRequests.Add(new Models.Kontakt.ContactRequest() {
                Date = DateTime.Now,
                Read = false,
                Content = content.ToString(),
                Subject = subject
            });

            db.SaveChanges();
            logger.Debug("The contact message was saved in the database, trying co nitify admin users!");

            //E-Mail an alle Administratoren versenden
            content.Append(@"<hr /><p>Diese Nachricht können Sie auch im Benachrichtigungen-Bereich der <a href='http://www.alisch-eventagentur.de'>Webseite</a> abrufen.</p>");
            
            SiteUser[] admins = db.SiteUsers.ToArray();

            foreach(SiteUser u in admins){
                bool success = EMailHelper.SendMail("kontakt@alisch-eventagentur.de", u.Email, "Neue Kontaktaufnahme über die Webseite", content.ToString(), true);
                if (!success)
                {
                    logger.Debug("Could not notify user " + u.Username);
                }
            }

            return RedirectToRoute(new { controller = "Site", action = "Show", id = 3 });
        }

    }
}
