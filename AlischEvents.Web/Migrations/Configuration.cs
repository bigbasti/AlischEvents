namespace AlischEvents.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using AlischEvents.Web.Models.Menu;


    internal sealed class Configuration : DbMigrationsConfiguration<AlischEvents.Web.Models.AlischDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(AlischEvents.Web.Models.AlischDB context)
        {
            //  This method will be called after migrating to the latest version.

            /* Wenn die Datenbak aktualisiert wird muss ein Men�eintrag f�r das Newsletter im Fu�bereicht der Seite
             * Angelegt werden. Fehlt dieser Link, k�nnen Besucher den Newsletter Bereich nicht aufrufen.
             * Dies wird nur ausgef�hrt, wenn noch kein Newsletter Men�eintrag vorhanden ist.
             */
            if (context.MenuEntries.FirstOrDefault(m => m.Title.Equals("Newsletter")) == null)
            {
                context.MenuEntries.Add(new MenuEntry()
                {
                    MenuOrderID = 3,
                    Position = 1,
                    Title = "Newsletter",
                    URL = "/Newsletter/Public",
                    MenuID = 1
                });
            }
        }
    }
}
