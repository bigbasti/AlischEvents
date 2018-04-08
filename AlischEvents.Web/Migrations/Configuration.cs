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

            /* Wenn die Datenbak aktualisiert wird muss ein Menüeintrag für das Newsletter im Fußbereicht der Seite
             * Angelegt werden. Fehlt dieser Link, können Besucher den Newsletter Bereich nicht aufrufen.
             * Dies wird nur ausgeführt, wenn noch kein Newsletter Menüeintrag vorhanden ist.
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
