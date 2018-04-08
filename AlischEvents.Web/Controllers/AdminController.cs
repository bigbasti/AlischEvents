using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using AlischEvents.Web.Security.Filters;
using AlischEvents.Web.Models.Security;
using AlischEvents.Web.Security.Membership;
using AlischEvents.Web.Models;
using AlischEvents.Web.Models.Admin;
using AlischEvents.Web.Security;
using log4net;


namespace AlischEvents.Web.Controllers
{
    public class AdminController : Controller
    {

        ILog logger = LogManager.GetLogger(typeof(AdminController));

        AlischDB db = new AlischDB(); 

        public MembershipProvider Membership; //Unschön but it works..
       // public ILoginProvider loginProvider;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext) {

            if (Membership == null) { Membership = new AlischMembershipProvider(); }
            //if (loginProvider == null) { loginProvider = new LoginProvider(); }

            base.Initialize(requestContext);
        }

        //
        // GET: /Admin/

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        //
        //GET: /Admin/Login

        public ActionResult Login() {
            return View();
        }

        //
        //POST: /Admin/Login

        [HttpPost]
        public ActionResult Login(LoginModel model) {

            if (!string.IsNullOrEmpty(model.Username) && !string.IsNullOrEmpty(model.Password)) {
                if (Membership.ValidateUser(model.Username, Hashing.GenerateMD5(model.Password))) {
                    logger.Debug("User " + model.Username + " was successfully logged in");
                    FormsAuthentication.SetAuthCookie(model.Username, false);
                    return RedirectToAction("Index", "Admin");
                } else {
                    logger.Debug("Could not log in user " + model.Username + " wrong password / username");
                    ViewData.ModelState.AddModelError("Error", "Der Benutzername oder das Passwort sind unbekannt!");
                }
            } else {
                logger.Debug("Not enough log in information provided - missing password or username!");
                //ViewData.ModelState.AddModelError("Username", "Bitte füllen Sie alle Felder aus!");
            }

            return View(model);
        }

        //
        //GET: /Admin/Logout

        public ActionResult Logout() {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        //GET: /Admin/Users

        [Authorize]
        public ActionResult Users() {

            return View(db.SiteUsers.Where(u => u.UserID != null));
        }
    }
}
