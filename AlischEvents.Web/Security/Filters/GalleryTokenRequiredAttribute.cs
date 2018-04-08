using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace AlischEvents.Web.Security.Filters {

    public class GalleryTokenRequiredAttribute : ActionFilterAttribute {

        ILog logger = LogManager.GetLogger(typeof(GalleryTokenRequiredAttribute));

        public override void OnActionExecuting(ActionExecutingContext filterContext) {

            AlischEvents.Web.Models.AlischDB db = new Models.AlischDB();

            if (!filterContext.HttpContext.Request.IsAuthenticated) {                               //Falls ein Benutzer eingeloggt ist wird kein Token benötigt
                string token = null;
                logger.Debug("No user is logged in, checking for access token...");
                if (filterContext.HttpContext.Request.Cookies.AllKeys.Contains("AccessToken")) {
                    token = filterContext.HttpContext.Request.Cookies["AccessToken"].Value;
                    logger.Debug("Found access tocken, checiking if its active...");

                    if (!db.AccessTokens.FirstOrDefault(t => t.Token.Equals(token)).IsActive) {
                        logger.Debug("Access Token is not active! Redirecting to error page!");
                        filterContext.HttpContext.Response.Redirect("~/Site/Show/7");
                    }
                } else {
                    logger.Debug("No Access token present! Redirecting to error page!");
                    filterContext.HttpContext.Response.Redirect("~/Site/Show/7");
                }
            }
        }
    }
}