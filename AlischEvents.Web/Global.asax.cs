using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using AlischEvents.Web.Models;
using AlischEvents.Web.Security.HttpHandler;

namespace AlischEvents.Web {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add("GalleryImagesRoute", new Route("galerie/{gallery}/{filename}", new GalleryImageRouteHandler()));

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            //Unbekannte/restliche Routen auf die 404 Seite umleiten
            routes.MapRoute(
                "Catch All",
                "{*path}",
                new { controller = "Home", action = "NotFound" }
            );
            
        }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            //Entity Framework - Datenbank neu generieren wenn Model Struktur geändert wurde
            Database.SetInitializer<AlischDB>(new AlischDBInitializer());

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            log4net.Config.XmlConfigurator.Configure();

        }
    }
}