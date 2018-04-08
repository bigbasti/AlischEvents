using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlischEvents.Web.Models.Galerie;
using AlischEvents.Web.Models;
using AlischEvents.Web.Security.Filters;
using log4net;

namespace AlischEvents.Web.Controllers
{ 
    public class GalleryController : Controller
    {
        ILog logger = LogManager.GetLogger(typeof(GalleryController));

        private AlischDB db = new AlischDB();

        //
        // GET: /Gallery/

        [Authorize]
        public ViewResult Index()
        {
            return View(db.Gallerys.ToList());
        }

        //
        // GET: /Gallery/Show/5

        [GalleryTokenRequired]
        public ActionResult Show(int id)
        {
            string tokenCode = "nicht nötig";
            AccessToken token = new AccessToken() { DateLimitation = DateTime.Now, Token = "Admin eingeloggt, daher kein Zugangscode nötig!" };

            if(!HttpContext.Request.IsAuthenticated){
                tokenCode = HttpContext.Request.Cookies["AccessToken"].Value;
                token = db.AccessTokens.FirstOrDefault(t => t.Token.Equals(tokenCode));
            }

            Gallery gallery = db.Gallerys.Find(id);

            string galleryPath = Server.MapPath("~/Content/galerie/" + gallery.FolderName);
            IEnumerable<string> picpaths = new LinkedList<string>();
            IEnumerable<string> filenames = new LinkedList<string>();
            try {
                filenames = System.IO.Directory.EnumerateFiles(galleryPath);
                picpaths = filenames.Select(f => Url.Content("~/galerie/" + gallery.FolderName + "/" + new System.IO.FileInfo(f).Name));
            } catch (Exception ex) {

            }

            ShowGalleryModel model = new ShowGalleryModel() {
                GalleryInfo = gallery,
                ImagePaths = picpaths,
                Token = token
            };

            return View(model);

        }

        //
        // GET: /Gallery/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Gallery/Create

        [Authorize]
        [HttpPost]
        public ActionResult Create(Gallery gallery)
        {
            if (ModelState.IsValid)
            {
                string path = Server.MapPath("~/Content/galerie/" + gallery.FolderName);

                try {
                    System.IO.Directory.CreateDirectory(path);
                } catch (Exception ex) {
                    //ViewData.ModelState.AddModelError("FolderName", "Der Ordner für die Galerie konnte nicht angelegt werden. Bitte kontaktieren Sie ihren Administrator um dieses Problem zu lösen. Detailierte Fehlermeldung: " + ex.Message);
                    //return View(gallery);
                    logger.Error("Could not create folder for gallery, user must create it manually!", ex);
                }


                db.Gallerys.Add(gallery);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(gallery);
        }
        
        //
        // GET: /Gallery/Edit/5

        [Authorize]
        public ActionResult Edit(int id)
        {
            Gallery gallery = db.Gallerys.Find(id);
            return View(gallery);
        }

        //
        // POST: /Gallery/Edit/5

        [Authorize]
        [HttpPost]
        public ActionResult Edit(Gallery gallery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gallery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gallery);
        }

        //
        // GET: /Gallery/Delete/5

        [Authorize]
        public ActionResult Delete(int id)
        {
            Gallery gallery = db.Gallerys.Find(id);
            return View(gallery);
        }

        //
        // POST: /Gallery/Delete/5

        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Gallery gallery = db.Gallerys.Find(id);

            string path = Server.MapPath("~/Content/galerie/" + gallery.FolderName);

            db.Gallerys.Remove(gallery);
            db.SaveChanges();

            logger.Debug("Gallery " + gallery.Title + " was successfully deleted");

            return RedirectToAction("Index");
        }

        //
        // GET: /Gallery/Remove/5?gal=6

        [Authorize]
        public ActionResult Remove(int id, int gal) {
            Gallery gallery = db.Gallerys.FirstOrDefault(g => g.GalleryID == gal);

            AccessToken token = db.AccessTokens.FirstOrDefault(t => t.TokenID == id);

            gallery.AccessTokens.Remove(token);
            db.SaveChanges();

            return RedirectToAction("Token", "Gallery", new { id = gal });
        }

        //
        // GET: /Gallery/About/5

        [Authorize]
        public ActionResult About(int id) {
            Gallery gallery = db.Gallerys.Find(id);

            string path = Server.MapPath("~/Content/galerie/" + gallery.FolderName);

            int files = 0;
            try {
                files = System.IO.Directory.GetFiles(path).Length;
            } catch (Exception ex) {
                logger.Error("Could not list folder contents", ex);
            }

            ViewBag.FilesInGallery = files;

            return View(gallery);
        }

        // Alle Token für eine Galerie anzeigen
        // GET: /Gallery/Token/5

        [Authorize]
        public ActionResult Token(int id) {
            Gallery gallery = db.Gallerys.Find(id);

            var tokens = db.AccessTokens.Where(t => t.IsActive);
            var galleries = new AlischDB().Gallerys.ToList();

            LinkedList<SelectListItem> tokenList = new LinkedList<SelectListItem>();

            //Nur ungenutzte Tokens anzeigen
            
            foreach (var tok in tokens) {
                bool isUsed = false;
                foreach (var gal in galleries) {
                    if (gal.AccessTokens.Where(t => t.Token.Equals(tok.Token)).ToList().Count > 0) {
                        isUsed = true;
                    }
                }
                if (!isUsed) {
                    tokenList.AddLast(new SelectListItem() {
                        Text = tok.Token,
                        Value = tok.TokenID.ToString()
                    });
                }
            }



            ViewBag.GalleryID = id;
            ViewBag.Tokens = tokenList;

            return View(gallery.AccessTokens);
        }

        // Ein Token zu einer Galerie hinzufügen, falls dieser Token schon vergeben ist wird er aus der Letzten Vergabe
        // Entfernt. Ein Token kann nur einer Galerie zugewiesen sein!
        // POST /Gallery/AddToken/4

        [Authorize]
        [HttpPost]
        public ActionResult AddToken(FormCollection collection) {

            string tokenToAdd = collection["Token"];
            string galleryID = collection["Gallery"];

            int tokenID;
            int galID;

            int.TryParse(tokenToAdd, out tokenID);
            int.TryParse(galleryID, out galID);

            Gallery gallery = db.Gallerys.FirstOrDefault(g => g.GalleryID == galID);
            AccessToken token = db.AccessTokens.FirstOrDefault(t => t.TokenID == tokenID);

            gallery.AccessTokens.AddLast(token);
            db.SaveChanges();

            return RedirectToAction("Token", new { id = galleryID });
        }

        // Falls ein gültiger Schlüssel angegeben wird wird ein Cookie gesetzt,
        // der für die nächsten Requests als Token genutzt wird
        // POST /Gallery/Login

        [HttpPost]
        public ActionResult Login(FormCollection collection) {
            string code = collection["Code"];

            AccessToken token = db.AccessTokens.FirstOrDefault(a => a.Token.Equals(code));
            if (token != null) {
                if (token.IsActive) {
                    //Prüfen, welche Galerien diesen Token nutzen
                    LinkedList<Gallery> foundGalleries = new LinkedList<Gallery>();
                    var galleries = db.Gallerys.ToList();
                    foreach (var gallery in galleries) {
                        if (gallery.AccessTokens.Where(t => t.Token.Equals(code)).ToList().Count > 0) {
                            foundGalleries.AddLast(gallery);
                        }
                    }
                    if (foundGalleries.Count > 0) {
                        HttpCookie cookie = new HttpCookie("AccessToken");
                        cookie.Value = code;
                        cookie.Expires = DateTime.Now.AddHours(1);
                        this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);

                        //Ein Token kann nur zu EINER Galerie gehören, daher hat die Auflistung nur einen Eintrag!
                        return RedirectToAction("Show", new { id = foundGalleries.ElementAt(0).GalleryID });
                    }
                    
                }
            }
            return RedirectToAction("Show", "Site", new { id = 7 });
        }

        /// <summary>
        /// Prüft, ob der vom Benutzer eingegebene Ordner schon von einer
        /// anderen Galerie genutzt wird oder noch von einer anderen (bereits
        /// gelöschten) Galerie noch üblich ist
        /// </summary>
        /// <param name="foldername">Der zu prüfende Ordnername</param>
        /// <returns>True wenn der Ordner noch nicht existiert</returns>
        [Authorize]
        public ActionResult FolderAvailable(string foldername) {
            string path = Server.MapPath("~/Content/galerie/" + foldername);

            if (System.IO.Directory.Exists(path)) {
                ViewData.ModelState.AddModelError("FolderName", "Dieser Ordnername wird bereits von einer anderen Galerie genutzt!");
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}