using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace AlischEvents.Web.Controllers
{
    public class ErrorController : Controller
    {
        //
        //GET: /Error/Unknown
        public ActionResult Unknown() {
            return View("Error");
        }

        //
        //GET: /Error/InternalServerError
        public ActionResult InternalServerError() {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ViewBag.Content = "<h3>500 - Interner Server Fehler aufgetreten</h3><p>Es ist zu einem Internen Fehler auf dem Server gekommen.<br /> Falls dies sich nicht ändert kontaktieren Sie bitte den Administrator.</p>";
            ViewBag.Title = "Interner Server Fehler aufgetreten";
            return View("Error");
        }

        //
        //GET: /Error/NotFound
        public ActionResult NotFound() {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            ViewBag.Content = "<h3>404 - Die gesuchte Seite wurde nicht gefunden</h3><p>Die Adresse, die Sie eingegeben haben ist nicht vorhanden oder ist falsch geschrieben.<br />Bitte überprüfen Sie nochmalls die Adresse.</p>";
            ViewBag.Title = "Gewünschter Inhalt nicht gefunden";
            return View("Error");
        }
    }
}
