using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlischEvents.Web.Models.Admin;
using AlischEvents.Web.Models;
using AlischEvents.Web.Models.Account;
using AlischEvents.Web.Security;
using AlischEvents.Web.Helpers;
using log4net;

namespace AlischEvents.Web.Controllers
{ 
    public class AccountController : Controller
    {
        ILog logger = LogManager.GetLogger(typeof(AccountController));

        private string new_pass_body = "<html><body><h3>Ihr neues Passwort</h3>" +
                                        "<p>ACHTING: Bitte antworten Sie nicht auf diese E-Mail. Wenn Sie weitere Fragen haben melden Sie sich bitte über unser Kontaktformular an uns!</p>" +
                                        "<p>Ihr neues Passwort lautet:<br />" +
                                            "<b>$NEWPASS$</b><br />" +
                                        "</p>" +
                                        "<p>Es wird empfohlen Ihr neues Passwort bei dem nächsten Login zu ändern!</p>" +
                                        "<hr><p>Alisch Eventagentur</p></body></html>";

        private AlischDB db = new AlischDB();

        //
        // GET: /Account/

        [Authorize]
        public ViewResult Index() {
            return View(db.SiteUsers.Where(u => u.UserID != null));
        }

        //
        // GET: /Account/Details/5

        [Authorize]
        public ViewResult Details(int id)
        {
            SiteUser siteuser = db.SiteUsers.Find(id);
            return View(siteuser);
        }

        //
        // GET: /Account/Create

        [Authorize]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Account/Create

        [Authorize]
        [HttpPost]
        public ActionResult Create(SiteUser siteuser)
        {
            if (ModelState.IsValid)
            {
                if (db.SiteUsers.Where(u => u.Username.Equals(siteuser.Username)).ToList().Count > 0) {
                    logger.Debug("The username is already used by another user");
                    ViewData.ModelState.AddModelError("Username", "Dieser Benutzername wird bereits benutzt!");
                    return View(siteuser);
                }
                logger.Info("New user added: " + siteuser.Username);
                db.SiteUsers.Add(siteuser);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(siteuser);
        }
        
        //
        // GET: /Account/Edit/5

        [Authorize]
        public ActionResult Edit(int id)
        {
            SiteUser siteuser = db.SiteUsers.Find(id);
            return View(siteuser);
        }

        //
        // POST: /Account/Edit/5

        [Authorize]
        [HttpPost]
        public ActionResult Edit(SiteUser siteuser)
        {
            try {
                SiteUser user = db.SiteUsers.FirstOrDefault(u => u.UserID == siteuser.UserID);

                user.Email = siteuser.Email;
                user.Firstname = siteuser.Firstname;
                user.Lastname = siteuser.Lastname;

                db.SaveChanges();
                return RedirectToAction("Index");
            } catch(Exception ex) {
                // Im Fehlerfall fortahren und Bearbeitungsmaske erneut anzeigen
            }
            return View(siteuser);
        }

        //
        // GET: /Account/Delete/5

        [Authorize]
        public ActionResult Delete(int id)
        {
            SiteUser siteuser = db.SiteUsers.Find(id);
            return View(siteuser);
        }

        //
        // POST: /Account/Delete/5

        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            SiteUser siteuser = db.SiteUsers.Find(id);
            db.SiteUsers.Remove(siteuser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Account/ChangePassword/5

        [Authorize]
        public ActionResult ChangePassword(int id) {

            ChangePasswordModel model = new ChangePasswordModel();
            model.AccountID = id;

            return View(model);
        }

        //
        // POST: /Account/ChangePassword/5

        [Authorize]
        [HttpPost, ActionName("ChangePassword")]
        public ActionResult ChangePasswordConfirmed(ChangePasswordModel model) {

            if (model.Pass1.Equals(model.Pass2)) {
                try {
                    SiteUser siteuser = db.SiteUsers.Find(model.AccountID);
                    siteuser.Password = Hashing.GenerateMD5(model.Pass1);
                    siteuser.Password2 = Hashing.GenerateMD5(model.Pass2);

                    db.SaveChanges();

                    logger.Debug("Password of user " + siteuser.Username + " was successfully cahnged");

                    return RedirectToAction("Edit", "Account", new { id = model.AccountID });
                } catch (Exception ex) {
                    logger.Error("Could not change password of user", ex);
                }
            }

            return View("ChangePassword", model);
        }

        //
        // GET: /Account/RestorePassword

        public ActionResult RestorePassword() {

            RestorePasswordModel model = new RestorePasswordModel();

            return View(model);
        }

        //
        // POST: /Account/RestorePassword

        [HttpPost, ActionName("RestorePassword")]
        public ActionResult RestorePasswordConfirmed(RestorePasswordModel model) {

            //Passwort-Reset
            SiteUser user = db.SiteUsers.FirstOrDefault(u => u.Email.Equals(model.Email));
            if (user != null) {
                //Es gibt einen Benutzer zu der angegebenen Emailadresse
                logger.Debug("Resetting password of user " + user.Username);

                //Zufallszahl als Passwort generieren
                string newPass = new Random().Next(1000, 1000000).ToString();

                //Versuchen das neue Passwort per Mail zu versenden
                if (EMailHelper.SendMail("noreply@alisch-eventagentur.de", model.Email, "Passwort Zurücksetzung", new_pass_body.Replace("$NEWPASS$", newPass), true)) {
                    //Wenn die Email erfolgreich gesendet wurde
                    user.Password = Hashing.GenerateMD5(newPass);
                    user.Password2 = user.Password;
                    db.SaveChanges();

                    logger.Debug("New password was successfully generated and sent to user");

                    return View("RestoreSuccessful");
                } else {
                    logger.Error("Could not sent the email containing the new password to user, the password was not changed!");
                    ViewData.ModelState.AddModelError("Email", "Es ist ein Fehler beim Versenden der Email aufgetreten. Bitte kontaktieren Sie den Administrator!");
                }

            } else {
                //Keinen Benutzer mit der Adresse gefunden
                logger.Debug("The given Email " + model.Email + " belongs to no registered user!");
                ViewData.ModelState.AddModelError("Email", "Es konnte kein Benutzer zu der angegebenen Adresse gefunden werden!");
            }

            return View("RestorePassword", model);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Login() {
            return RedirectToAction("Login", "Admin");
        }
    }
}