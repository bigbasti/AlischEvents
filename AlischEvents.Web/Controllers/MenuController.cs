using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlischEvents.Web.Models;
using AlischEvents.Web.Models.Site;
using AlischEvents.Web.Models.Menu;

namespace AlischEvents.Web.Controllers
{
    public class MenuController : Controller
    {
        //
        // GET: /Menu/

        [Authorize]
        public ActionResult Index()
        {
            AlischDB db = new AlischDB();

            var menu = db.BlogMenus.Where(i => i.MenuID == 1);
            var test = menu.ToList().ElementAt(0);
            var menuEntries = test.MenuEntries.Where(e => e.Position == 0).OrderBy(i => i.MenuOrderID);

            return View(menuEntries);
        }


        //
        //GET: Menu/Move/1&site=$

        [Authorize]
        public ActionResult Move(int id, int to) {

            AlischDB db = new AlischDB();

            //Prüfen, ob übergebene ids Blogs oder Webseiten sind
            MenuEntry toMove = db.MenuEntries.FirstOrDefault(w => w.MenuOrderID == to && w.Position == 0);

            MenuEntry moveTo = db.MenuEntries.FirstOrDefault(w => w.MenuOrderID == id && w.Position == 0);

            int oldMenu = toMove.MenuOrderID;

            toMove.MenuOrderID = id;
            moveTo.MenuOrderID = oldMenu;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Create() {
            return View();
        }
    }
}
