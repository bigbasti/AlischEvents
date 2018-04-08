using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlischEvents.Web.Models.Blog;
using AlischEvents.Web.Models;
using AlischEvents.Web.Models.Menu;
using log4net;

namespace AlischEvents.Web.Controllers
{ 
    public class BlogController : Controller
    {
        ILog logger = LogManager.GetLogger(typeof(BlogController));

        private AlischDB db = new AlischDB();

        //
        // GET: /Blog/
        [Authorize]
        public ViewResult Index()
        {
            return View(db.Blogs.ToList());
        }

        //
        // GET: /Blog/Show/5
        
        public ViewResult Show(int id)
        {
            Blog blog = db.Blogs.Find(id);
            return View(blog);
        }

        //
        // GET: /Blog/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Blog/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.Posts = new LinkedList<BlogPost>();

                db.Blogs.Add(blog);
                db.SaveChanges();

                //MenuorderID zuweisen
                var menu = db.BlogMenus.Where(i => i.MenuID == 1).ToList().ElementAt(0);

                int entriesCount = menu.MenuEntries.Where(e => e.Position == 0).ToList().Count;
                menu.MenuEntries.AddLast(new Models.Menu.MenuEntry() {
                    MenuOrderID = entriesCount + 1,
                    Position = 0,
                    Title = blog.Name,
                    URL = "/Blog/Show/" + blog.BlogID
                });

                blog.MenuOrderID = 0;

                db.SaveChanges();

                logger.Debug("Created new Blog " + blog.Name);

                return RedirectToAction("Index");  
            }

            return View(blog);
        }
        
        //
        // GET: /Blog/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            Blog blog = db.Blogs.Find(id);
            return View(blog);
        }

        //
        // POST: /Blog/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(Blog blog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        //
        // GET: /Blog/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            Blog blog = db.Blogs.Find(id);
            return View(blog);
        }

        //
        // POST: /Blog/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Blog blog = db.Blogs.Find(id);
            db.Blogs.Remove(blog);
            db.SaveChanges();
            logger.Debug("Blog " + blog.Name + " successfully deleted");

            //Menüeintrag entfernen falls vorhanden
            var menu = db.BlogMenus.Where(i => i.MenuID == 1).ToList().ElementAt(0);

            logger.Debug("Checking if there is an associated menu to the deleted blog...");
            string url = "/Blog/Show/" + id;
            MenuEntry entry = menu.MenuEntries.FirstOrDefault(e => e.URL.Equals(url));
            if (entry != null) {
                logger.Debug("Found menu entry, deleting...");
                db.MenuEntries.Remove(entry);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //
        // GET: /Blog/Posts/5
        [Authorize]
        public ActionResult Posts(int id) {

            Blog blog = db.Blogs.Find(id);

            ViewBag.BlogTitle = blog.Title;
            ViewBag.BlogID = blog.BlogID;

            IEnumerable<BlogPost> posts = blog.Posts;
            List<BlogPost> retVal = (posts == null) ? new List<BlogPost>() : posts.ToList();
            return View(retVal);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}