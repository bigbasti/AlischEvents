using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlischEvents.Web.Models.Site;
using AlischEvents.Web.Models;
using System.IO;
using AlischEvents.Web.Models.Menu;
using log4net;

namespace AlischEvents.Web.Controllers
{ 
    public class SiteController : Controller
    {
        ILog logger = LogManager.GetLogger(typeof(SiteController));

        private AlischDB db = new AlischDB();

        //
        // GET: /Site/
        [Authorize]
        public ViewResult Index()
        {
            return View(db.WebSites.Where(s => s.IsStaticSite == false).ToList());
        }

        //
        // GET: /Site/Show/5

        public ViewResult Show(int id)
        {
            WebSite website = db.WebSites.Find(id);
            return View(website);
        }

        //
        // GET: /Site/Create
        [Authorize]

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Site/Create

        [Authorize]
        [HttpPost]
        public ActionResult Create(WebSite website)
        {
            if (ModelState.IsValid)
            {
                if (db.WebSites.Where(w => w.SiteLabel.Equals(website.SiteLabel)).ToList().Count > 0) {
                    ViewData.ModelState.AddModelError("SiteLabel", "Es Existiert bereits eine Seite mit dieser Bezeichnung");
                    logger.Debug("The label " + website.SiteLabel + " already exists!");
                    return View(website);
                }
                
                website.SiteContent = "kein Inhalt";
                website.IsStaticSite = false;           //Statische Seiten können nicht vom Benutzer erstellt werden
                website.SiteURL = "";                   //nur relevant für statische Seiten
                db.WebSites.Add(website);   
                db.SaveChanges();

                if (website.ShowOnMenu) {
                    //MenuorderID zuweisen
                    var menu = db.BlogMenus.Where(i => i.MenuID == 1).ToList().ElementAt(0);
                    
                    int entriesCount = menu.MenuEntries.Where(e => e.Position == 0).ToList().Count;
                    menu.MenuEntries.AddLast(new Models.Menu.MenuEntry() {
                        MenuOrderID = entriesCount + 1,
                        Position = 0,
                        Title = website.SiteLabel,
                        URL = "/Site/Show/" + website.SiteID
                    });

                    website.MenuOrderID = 0;

                    db.SaveChanges();
                }
                return RedirectToAction("Index");  
            }

            return View(website);
        }
        
        //
        // GET: /Site/Edit/5

        [Authorize]
        public ActionResult Edit(int id)
        {
            WebSite website = db.WebSites.Find(id);
            return View(website);
        }

        //
        // POST: /Site/Edit/5

        [Authorize]
        [HttpPost]
        public ActionResult Edit(WebSite website, int id)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(website).State = EntityState.Modified;
                var site = db.WebSites.Single(s => s.SiteID == id);

                try {
                    //Den Menüeintrag anpassen (kann sich geändert haben)
                    var menu = db.BlogMenus.Where(i => i.MenuID == 1).ToList().ElementAt(0);
                    MenuEntry entry = menu.MenuEntries.FirstOrDefault(e => e.Title.Equals(site.SiteLabel));
                    entry.Title = website.SiteLabel;
                } catch (Exception ex) { }

                site.SiteContent = website.SiteContent;
                site.ShowOnMenu = website.ShowOnMenu;
                site.SiteTitle = website.SiteTitle;
                site.SiteLabel = website.SiteLabel;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(website);
        }

        //
        // GET: /Site/Delete/5

        [Authorize]
        public ActionResult Delete(int id)
        {
            WebSite website = db.WebSites.Find(id);
            return View(website);
        }

        //
        // GET: /Site/Static

        [Authorize]
        public ActionResult Static() {
            return View(db.WebSites.Where(s => s.IsStaticSite).ToList());
        }

        //
        // POST: /Site/Delete/5

        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            WebSite website = db.WebSites.Find(id);
            db.WebSites.Remove(website);
            db.SaveChanges();

            //Menüeintrag entfernen falls vorhanden
            var menu = db.BlogMenus.Where(i => i.MenuID == 1).ToList().ElementAt(0);

            string url = "/Site/Show/" + id;
            MenuEntry entry = menu.MenuEntries.FirstOrDefault(e => e.URL.Equals(url));
            if (entry != null) {
                db.MenuEntries.Remove(entry);
            }

            db.SaveChanges();

            logger.Debug("Site " + website.SiteLabel + " was successfully deleted!");
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Wird vom CKEditor aufgerufen wenn eine Datei hochgeladen werden soll
        /// </summary>
        /// <param name="upload"></param>
        /// <param name="CKEditorFuncNum"></param>
        /// <param name="CKEditor"></param>
        /// <param name="langCode"></param>
        /// <returns></returns>

        [Authorize]
        [HttpPost]
        public ActionResult UploadImage(string CKEditorFuncNum, string CKEditor, string langCode) {
            string url; // url to return
            string message; // message to display (optional)

            var file = Request.Files[0];

            var fileName = Path.GetFileName(file.FileName);

            string output = "";

            if (file.ContentLength > 0) {
                try {
                    
                    var path = Path.Combine(Server.MapPath("~/Content/uploads/artikel/grafiken/"), fileName);
                    file.SaveAs(path);
                    logger.Debug("New File was uploaded " + fileName + " and was stored at " + path);
                } catch (Exception ex) {
                    message = ex.Message + "\n\r" + ex.StackTrace;
                    logger.Error("Could not store uploaded file", ex);
                    output = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + "http://localhost:1457/Content/Images/my_uploaded_image.jpg" + "\", \"" + message + "\");</script></body></html>";
                    return Content(output);
                }
            }

            // path of the image
            
            string pathToFile = Url.Content("~/Content/uploads/artikel/grafiken/") + fileName;

            // will create http://localhost:1457/Content/Images/my_uploaded_image.jpg
            url = Request.Url.GetLeftPart(UriPartial.Authority) + pathToFile;

            // passing message success/failure
            message = "Das Bild wurde erfolgreich hochgeladen :)";

            // since it is an ajax request it requires this string
            output = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\", \"" + message + "\");</script></body></html>";
            return Content(output);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public string web { get; set; }
    }
}