using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlischEvents.Web.Models.Newsletter;
using AlischEvents.Web.Models;
using AlischEvents.Web.Helpers;
using System.Text.RegularExpressions;
using log4net;

namespace AlischEvents.Web.Controllers
{
    public class NewsletterController : Controller
    {
        ILog logger = LogManager.GetLogger(typeof (NewsletterController));

        //
        // GET: /Newsletter/

        [Authorize]
        public ActionResult Index()
        {
            AlischDB db = new AlischDB();

            IEnumerable<Newsletter> lastFiveNewsletters = db.Newsletters.OrderByDescending(o => o.Id).Take(5);

            return View(lastFiveNewsletters);
        }

        //
        // GET: /Newsletter/Public

        public ActionResult Public()
        {
            return View();
        }

        //
        // GET: /Newsletter/Confirm/$confirmationid

        public ActionResult Confirm(int id)
        {
            AlischDB db = new AlischDB();

            NewsletterConsumer uc = db.NewsletterConsumers.FirstOrDefault(c => c.EmailsReceived == (id*-1));
            if(uc != null)
            {
                uc.EmailsReceived = 0; //Benutzer darf nun Newsletter empfangen
                db.SaveChanges();
                logger.Error("User " + uc.Email + " successfully confirmed his Emailadress");

                return View("ConfirmSuccess");      //Alles ok
            }else
            {
                logger.Error("Confirmation failed because there is no User wating with ID " + id);
            }

            //Benutzer nicht gefunden -> auf startseite umleiten
            return RedirectToAction("Index");
        }

        // Wenn eine id übergeben wird soll das übergebene Newsletter zum bearbeiten geöffnet werden
        // GET: /Newsletter/Create{/$id}

        [Authorize]
        public ActionResult Create(int? id)
        {
            if (id != null)
            {
                AlischDB db = new AlischDB();

                Newsletter edit = db.Newsletters.FirstOrDefault(n => n.Id == id);

                if (edit != null)
                {
                    return View(edit);
                }
            }

            return View();
        }

        // GET: /Newsletter/Details/$id

        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id != null)
            {
                AlischDB db = new AlischDB();

                Newsletter edit = db.Newsletters.FirstOrDefault(n => n.Id == id);

                if (edit != null)
                {
                    return View(edit);
                }
            }

            return RedirectToAction("Overview");
        }

        // GET: /Newsletter/User

        [Authorize]
        public ActionResult User()
        {
            AlischDB db = new AlischDB();

            IEnumerable<NewsletterConsumer> users = db.NewsletterConsumers;

            return View(users);
        }

        // GET: /Newsletter/Overview

        [Authorize]
        public ActionResult Overview()
        {
            AlischDB db = new AlischDB();

            IEnumerable<Newsletter> nl = db.Newsletters;

            return View(nl);
        }

        // POST: /Newsletter/CheckEmail/$emailaddress

        public ActionResult CheckEmail(FormCollection collection)
        {
            try
            {
                if (collection[0] != null)
                {
                    AlischDB db = new AlischDB();

                    string email = collection[0];

                    logger.Debug("Checking Email-Address format of " + email);

                    string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                            + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                            + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                            + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                            + @"[a-zA-Z]{2,}))$";
                    Regex reStrict = new Regex(patternStrict);

                    bool isStrictMatch = reStrict.IsMatch(email);
                    if (!isStrictMatch)
                    {
                        logger.Error("The EMail address " + email + " does not match the required format somebody@email.com");
                        return View("Public", collection);
                    }

                    NewsletterConsumer consumer = db.NewsletterConsumers.FirstOrDefault(c => c.Email.Equals(email));

                    if (consumer != null)
                    {
                        //Dieser Kunde ist bereits registriert => Mitgliedschaft umkehren
                        logger.Debug("Consumer is already registered changing activity to: " + !consumer.Active);
                        consumer.Active = !consumer.Active;

                        //Wenn Benutzer sich wieder abmeldet muss dies dem Benutzer per E-Mail bestätigt werden
                        if(consumer.Active == false)
                        {
                            //Neuer Status = false
                            try
                            {
                                String email_content =
                                    "<html><head><title>Ihr Newsletter wurde erfolgreich abbestellt</title></head><body><h1>Sie erhalten keine weiteren Newsletter</h1><p>Sie haben Sich soeben erfolgreich von unserem Newsletter abgemeldet.</p><p>Das finden wir sehr schade. Sie können es sich aber natürlich jederzeit anders überlegen, unsere Tür steht Ihnen immer offen!</p><hr /><p>Ihr Team der Eventagentur Alisch</p></body></html>";
                                EMailHelper.SendMail("info@alisch-eventagentur.de", consumer.Email, "Ihr Newsletter wurde erfolgreich abbestellt", email_content, true);
                            }catch(Exception ex)
                            {
                                logger.Error("Could not sent the confirmation for unregistering user " + consumer.Email);
                            }
                        }
                    }
                    else
                    {
                        logger.Debug("Registering new consumer: " + email);
                        //Kunde registreirt sich zum ersten mal
                        int userkey = new Random().Next(1000, 1000000);
                        consumer = new NewsletterConsumer()
                        {
                            Active = true,
                            Email = collection[0],
                            EmailsReceived = userkey * -1,            //Benutzer muss erst den Bestätigungslink klicken
                        };
                        db.NewsletterConsumers.Add(consumer);

                        //Dem Benutzer eine Bestätigungsmail senden, mit einem Link zum Bestätigen
                        //Erst wenn der Benutzer diesen Link geklickt hat bekommt er auch Newsletter
                        try
                        {
                            String email_content =
                                "<html><head><title>Willkommen bei unserem Newsletter</title></head><body><h1>Willkommen bei unserem Newsletter</h1><p>Wir freuen uns sehr, dass Sie sich entschieden haben unseren Newsletter zu erhalten.</p><p>Freuen Sie sich auf viele Informationen über zukünftige Events und weitere spannende Aktionen</p><p><b>Anmeldung bestätigen</b></p><p>Bitte klicken Sie auf den folgenden Link um Ihre E-Mailadresse zu bestätigen und für den Empfang von Newslettern freizuschalten:</p><p><a href='http://alisch-eventagentur.de/Newsletter/Confirm/" + userkey + "'>http://alisch-eventagentur.de/Newsletter/Confirm/" + userkey + "</a></p><hr /><p>Ihr Team der Eventagentur Alisch</p></body></html>";
                            EMailHelper.SendMail("info@alisch-eventagentur.de", consumer.Email, "Willkommen bei unserem Newsletter", email_content, true);
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Could not sent the confirmation for registering user " + consumer.Email);
                        }
                    }

                    ViewBag.Active = consumer.Active;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error while checking consumer status:", ex);
                return View("Public");
            }

            return View("Confirm");
        }

        // POST: /Newsletter/Save/$Newsletter

        [Authorize]
        [HttpPost]
        public ActionResult Save(Newsletter newsletter)
        {
            if (newsletter != null && !string.IsNullOrEmpty(newsletter.Subject) &&
                                      !string.IsNullOrEmpty(newsletter.Content))
            {

                AlischDB db = new AlischDB();

                Newsletter edit = db.Newsletters.FirstOrDefault(n => n.Id == newsletter.Id);

                if (edit != null)
                {
                    //Es ist ein update
                    edit.Subject = newsletter.Subject;
                    edit.Content = newsletter.Content;
                    edit.HtmlContent = true;
                    edit.Date = DateTime.Now;

                }
                else
                {
                    //Neuer newsletter wurde angelegt
                    newsletter.Date = DateTime.Now;
                    newsletter.HtmlContent = true;
                    db.Newsletters.Add(newsletter);
                }

                db.SaveChanges();

                return RedirectToAction("Test", new { Id = newsletter.Id });
            }

            return View("Create", newsletter);
        }

        // GET: /Newsletter/Test/$id

        [Authorize]
        public ActionResult Test(int? id)
        {
            if (id != null)
            {
                AlischDB db = new AlischDB();

                Newsletter test = db.Newsletters.FirstOrDefault(n => n.Id == id);

                if (test != null)
                {
                    return View(test);
                }
            }

            return RedirectToAction("Index");
        }


        // GET: /Newsletter/SendTestLetter/$emailaddress

        [Authorize]
        public string SendTestLetter(string id, string nid)
        {
            if (id != null)
            {
                AlischDB db = new AlischDB();

                int int_nid = Convert.ToInt32(nid);

                Newsletter test = db.Newsletters.FirstOrDefault(n => n.Id == int_nid);

                if (test != null)
                {
                    logger.Debug("Trying to send a test Newsletter to " + id);

                    bool success = EMailHelper.SendMail("info@alisch-eventagentur.de", id, test.Subject, test.Content, test.HtmlContent);
                    if (success)
                    {
                        logger.Debug("Test Newsletter successfully sent");
                        return "true";
                    }
                }
            }

            Response.StatusCode = 501;
            return "false";
        }

        // GET: /Newsletter/SendNewsletter/$newsletterid

        [Authorize]
        public string SendNewsletter(int id)
        {
            AlischDB db = new AlischDB();

            Newsletter newsletter = db.Newsletters.FirstOrDefault(n => n.Id == id);


            if (newsletter != null)
            {
                logger.Debug("Sending Newsletter to all active consumers...");
                int count = 0;
                foreach (NewsletterConsumer consumer in db.NewsletterConsumers.Where(c => c.Active && c.EmailsReceived >= 0).ToList())
                {   //c.EmailsReceived >= 0 -> Alle Consumer die den Bestätigungslink geklickt haben
                    try
                    {
                        logger.Debug("Sending Email to " + consumer.Email);
                        EMailHelper.SendMail("info@alisch-eventagentur.de", consumer.Email, newsletter.Subject, newsletter.Content, newsletter.HtmlContent);
                        
                        consumer.EmailsReceived++;
                        count++;
                    }
                    catch (Exception ex)
                    {}
                    //newsletter.Recipients = new List<NewsletterConsumer>();
                }

                logger.Debug("Successfully sent out " + count + " Emails");

                newsletter.Recipients = count;
                newsletter.WasSent = true;
                db.SaveChanges();
                
                return "true";
            }

            Response.StatusCode = 501;
            return "false";
        }

    }
}
