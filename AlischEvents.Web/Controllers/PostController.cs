using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlischEvents.Web.Models.Blog;
using AlischEvents.Web.Models;
using log4net;

namespace AlischEvents.Web.Controllers
{ 
    [HandleError]
    public class PostController : Controller
    {
        ILog logger = LogManager.GetLogger(typeof(PostController));

        private AlischDB db = new AlischDB();

        //
        // GET: /Post/
        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Blog");
        }

        //
        // GET: /Post/Details/5

        public ActionResult Show(int id)
        {
            BlogPost blogpost = db.BlogsPosts.Find(id);
            if (blogpost != null) {
                if (blogpost.Published) {
                    return View(blogpost);
                }
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Post/Create/id
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Die ID des Blogs zu dem der neue Artikel gehören soll</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Create(int id)
        {
            Blog blog = db.Blogs.Find(id);

            ViewBag.BlogTitle = blog.Title;
            ViewBag.BlogID = id.ToString();

            BlogPost post = new BlogPost() {
                BlogID = id,
                Date = DateTime.Now
            };


            return View(post);
        } 

        //
        // POST: /Post/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(BlogPost blogpost)
        {
            if (ModelState.IsValid)
            {
                Blog blog = db.Blogs.Find(blogpost.BlogID);
                blogpost.Content = "Kein Inhalt";
                blogpost.PreviewImage = "";
                blogpost.PreviewText = "bitte ausfüllen";
                blogpost.Published = false;
                blogpost.Comments = new LinkedList<PostComment>();

                blog.Posts.Add(blogpost);

                db.SaveChanges();

                logger.Debug("Created new Blogpost " + blogpost.Title);
                return RedirectToAction("Edit", new { id = blogpost.PostID });  
            }

            return View(blogpost);
        }
        
        //
        // GET: /Post/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            BlogPost blogpost = db.BlogsPosts.Find(id);
            return View(blogpost);
        }

        //
        // POST: /Post/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(BlogPost blogpost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blogpost).State = EntityState.Modified;
                BlogPost post = db.BlogsPosts.Find(blogpost.PostID);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogpost);
        }

        //
        // GET: /Post/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            BlogPost blogpost = db.BlogsPosts.Find(id);
            return View(blogpost);
        }

        //
        // POST: /Post/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            BlogPost blogpost = db.BlogsPosts.Find(id);
            db.BlogsPosts.Remove(blogpost);
            db.SaveChanges();
            logger.Debug("Deleted Blogpost " + blogpost.Title);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}