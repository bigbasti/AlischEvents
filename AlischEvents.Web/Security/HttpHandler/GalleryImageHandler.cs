using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.IO;
using AlischEvents.Web.Models.Galerie;
using log4net;

namespace AlischEvents.Web.Security.HttpHandler
{
    /// <summary>
    /// Sorgt dafür dass der Ordner mit dem Galerie-Bildern nur erreichbar ist,
    /// wenn der passende Code genutzt wurde
    /// </summary>
    public class GalleryImageHandler : IHttpHandler
    {
        static ILog logger = LogManager.GetLogger(typeof(GalleryImageHandler));

        public GalleryImageHandler(RequestContext context)
        {
            ProcessRequest(context);
        }

        private static void ProcessRequest(RequestContext requestContext)
        {
            AlischEvents.Web.Models.AlischDB db = new Models.AlischDB();
           
            var response = requestContext.HttpContext.Response;
            var request = requestContext.HttpContext.Request;
            var server = requestContext.HttpContext.Server;

            string token = null;
            if (request.Cookies.AllKeys.Contains("AccessToken")) {
                token = request.Cookies["AccessToken"].Value;
            }

            string gallery = requestContext.RouteData.Values["gallery"].ToString();
            var filename = requestContext.RouteData.Values["filename"].ToString();

            var path = server.MapPath("~/Content/galerie/" + gallery + "/");

            //Wenn ein Administrator eingeloggt ist, das Bild ausliefern.
            if (request.IsAuthenticated) {
                logger.Debug("Admin user (" + request.RequestContext.HttpContext.User.Identity.Name + ") requests access to gallery " + gallery + " -> ok!");
                GrantAccessToFile(request, response, path + filename);
                return;
            }

            // Requirement Version 1.1: Facebookaccess
            // ---------------------------------------
            // Wenn Facebook auf ein Bild zugreift um es z.B. auf einer Benutzer-Wall
            // darzustellen soll dies erlaubt werden.
            // Wenn der Benutzer dann dieses Bild (auf seiner Wall) anklickt bekommt er
            // aber keinen Zugriff auf das Bild und wird zur Fehlerseite umgeleitet
            // ---------------------------------------
            // Dies hat den Sinn, dass Besucher der Galerien Bilder auf Facebook teilen
            // können, aber nicht den vollen Zugriff auf die Galerie freigeben.
            try
            {
                //Referrer  -> um das Bild in Groß zu öffnen
                if (request.UrlReferrer.Host.ToLower().Contains("facebook"))
                {
                    logger.Debug("User from Facebook requests a picture from gallery " + gallery);
                    //Darstellen des Bildes auf der Wall erlauben
                    GrantAccessToFile(request, response, path + filename);
                    return;
                }
            }
            catch (Exception ex) {
                logger.Error("Error while checking for facebook request", ex);
            }

            try
            {
                //Useragent -> Um Bild auf der Wall zu zeigen
                if(request.UserAgent.ToLower().Contains("facebook")){
                    logger.Debug("Facebook requests a picture from gallery " + gallery);
                    //Darstellen des Bildes auf der Wall erlauben
                    GrantAccessToFile(request, response, path + filename);
                    return;
                }
            }
            catch (Exception ex) {
                logger.Error("Error while checiking for facebook request", ex);
            }


            if (token != null) {
                //Es wurde ein AccessToken verwendet

                //Prüfen, ob es dieses AccessToken gibt
                if (db.AccessTokens.Where(t => t.Token.Equals(token)).ToList().Count > 0) {
                    //Es wurde ein Eintrag gefunden, prüfen, ob dieser Eintrag gültig ist
                    AccessToken at = db.AccessTokens.FirstOrDefault(t => t.Token.Equals(token));

                    if(TokenIsValid(at)){
                        //Wenn das Token auf eine Datei beschränkt ist diese nun ausgeben
                        if(at.FileLimitation != null){
                            var content = at.FileLimitation.Split('/');
                            if(gallery.Equals(content[0]) && filename.Equals(content[1])){
                                //Schlüssel passt zu der angeforderten URL

                                GrantAccessToFile(request, response, path + filename);
                            }else{
                                DeclineAccessToFile(request, response);
                                logger.Debug("The used token was not found in the gallery!");
                            }
                        }else{
                            //Token nicht nur auf eine Datei beschränkt

                            Gallery g = db.Gallerys.FirstOrDefault(gal => gal.FolderName.Equals(gallery));
                            if(g != null){
                                //Es wurde eine Galerie zu der angegeben URL gefunden
                                if(g.AccessTokens.Contains(at)){
                                    //Das genutzte AccessToken ist für die angeforderte Galerie freigeschaltet

                                    GrantAccessToFile(request, response, path + filename);

                                }else{
                                    logger.Debug("The used token was not found in the gallery!");
                                    DeclineAccessToFile(request, response);
                                }
                            }else{
                                logger.Debug("No Gallery was found to the provided token!");
                                DeclineAccessToFile(request, response);
                            }
                        }
                    }else{
                        logger.Debug("The used token is not active anymore!");
                        //Token bereits abgelaufen
                        DeclineAccessToFile(request, response);
                    }
                } else {
                    logger.Debug("The used token can not be found (anymore)");
                    //Es wurde ein unbekanntes Token angegeben
                    DeclineAccessToFile(request, response);
                }

            }else{
                logger.Debug("No token was provided access to gallery declined!");
                //Es wurde kein Token angegeben
                DeclineAccessToFile(request, response);
            }
        }

        private static void GrantAccessToFile(HttpRequestBase request, HttpResponseBase response, string pathToFile) {
            response.Clear();
            response.ContentType = GetContentType(request.Url.ToString());

            response.TransmitFile(pathToFile);
            response.End();
        }

        private static void DeclineAccessToFile(HttpRequestBase request, HttpResponseBase response) {
            response.Clear();
            response.ContentType = GetContentType(request.Url.ToString());
            response.StatusCode = 403; //Forbidden

            response.Redirect("/Site/Show/7");
            response.End();
        }

        /// <summary>
        /// Prüft ein angegebenes AccessToken auf Gültigkeit
        /// </summary>
        /// <param name="at">Das zu prüfende AccessToken</param>
        /// <returns>True wenn das AccessToken gültig ist</returns>
        private static bool TokenIsValid(AccessToken at) {
            AlischEvents.Web.Models.AlischDB db = new Models.AlischDB();

            at = db.AccessTokens.FirstOrDefault(t => t.TokenID == at.TokenID);

            if (at == null) {
                return false;
            }
            if (at.IsActive) {
                //Token wurde noch noch als ungültig markiert, prüfen ob das noch passt

                bool activeCheck = true;

                //Klick Limitierung prüfen
                if (at.ClickLimitation == 0) {
                    //Klicklimitierung ist abgelaufen
                    activeCheck = false; 
                }

                //Datums Limitierung prüfen
                if (at.DateLimitation <= DateTime.Now) {
                    //Datum überschritten
                    activeCheck = false;
                }

                if (!activeCheck) {
                    //Als deaktiviert markieren für den nöchsten Aufruf
                    at.IsActive = false;
                    db.SaveChanges();
                    return false;
                } else {
                    if (at.ClickLimitation > 0) {
                        at.ClickLimitation--;
                        db.SaveChanges();
                    }

                    return true;
                }
            } else {
                return false;
            }
        }

        /// <summary>
        /// Gibt den mime-Typ der angeforderten Ressource an Hand der Endung wieder
        /// </summary>
        /// <param name="url">Url zu der Ressource</param>
        /// <returns>Der Mime-Typ</returns>
        private static string GetContentType(string url)
        {
            switch (Path.GetExtension(url))
            {
                case ".gif":
                    return "Image/gif";
                case ".jpg":
                    return "Image/jpeg";
                case ".jpeg":
                    return "Image/jpeg";
                case ".png":
                    return "Image/png";
                default:
                    break;
            }
            return null;
        }

        public void ProcessRequest(HttpContext context)
        {
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}