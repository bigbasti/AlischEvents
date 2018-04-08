using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlischEvents.Web.Models.Kontakt;
using AlischEvents.Web.Models;

namespace AlischEvents.Web.Controllers
{ 
    public class NotificationsController : Controller
    {
        private AlischDB db = new AlischDB();

        //
        // GET: /Notifications/
        [Authorize]
        public ViewResult Index()
        {
            return View(db.ContactRequests.ToList());
        }

        //
        // GET: /Notifications/Details/5
        [Authorize]
        public ViewResult Show(int id)
        {
            ContactRequest contactrequest = db.ContactRequests.Find(id);
            contactrequest.Read = true;
            db.SaveChanges();

            return View(contactrequest);
        }

        //
        // GET: /Notifications/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // GET: /Notifications/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            ContactRequest contactrequest = db.ContactRequests.Find(id);
            return View(contactrequest);
        }

        //
        // POST: /Notifications/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            ContactRequest contactrequest = db.ContactRequests.Find(id);
            db.ContactRequests.Remove(contactrequest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}