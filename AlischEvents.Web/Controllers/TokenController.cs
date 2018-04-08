using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlischEvents.Web.Models.Galerie;
using AlischEvents.Web.Models;

namespace AlischEvents.Web.Controllers
{ 
    public class TokenController : Controller
    {
        private AlischDB db = new AlischDB();

        //
        // GET: /Token/

        [Authorize]
        public ViewResult Index()
        {
            return View(db.AccessTokens.ToList());
        }

        //
        // GET: /Token/Details/5

        [Authorize]
        public ViewResult Show(int id)
        {
            AccessToken accesstoken = db.AccessTokens.Find(id);
            return View(accesstoken);
        }

        //
        // GET: /Token/Create

        [Authorize]
        public ActionResult Create()
        {
            AccessToken token = new AccessToken() {
                ClickLimitation = -1,
                DateLimitation = DateTime.Now,
                IsActive = true
            };
            return View(token);
        } 

        //
        // POST: /Token/Create

        [Authorize]
        [HttpPost]
        public ActionResult Create(AccessToken accesstoken)
        {
            if (ModelState.IsValid)
            {
                db.AccessTokens.Add(accesstoken);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(accesstoken);
        }
        
        //
        // GET: /Token/Edit/5

        [Authorize]
        public ActionResult Edit(int id)
        {
            AccessToken accesstoken = db.AccessTokens.Find(id);
            return View(accesstoken);
        }

        //
        // POST: /Token/Edit/5

        [Authorize]
        [HttpPost]
        public ActionResult Edit(AccessToken accesstoken)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accesstoken).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accesstoken);
        }

        //
        // GET: /Token/Delete/5

        [Authorize]
        public ActionResult Delete(int id)
        {
            AccessToken accesstoken = db.AccessTokens.Find(id);
            return View(accesstoken);
        }

        //
        // POST: /Token/Delete/5

        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            AccessToken accesstoken = db.AccessTokens.Find(id);
            db.AccessTokens.Remove(accesstoken);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // POST: /Token/Toggle/5

        [Authorize]
        public ActionResult Toggle(int id) {
            AccessToken accesstoken = db.AccessTokens.Find(id);

            accesstoken.IsActive = !accesstoken.IsActive;

            db.SaveChanges();

            string referrer = HttpContext.Request.UrlReferrer.AbsoluteUri;

            return Redirect(referrer);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}