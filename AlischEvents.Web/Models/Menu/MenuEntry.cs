using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlischEvents.Web.Models.Menu {
    public class MenuEntry {

        [Key]
        public int MenuEntryID { get; set; }

        public int MenuID { get; set; }

        [DisplayName("Entscheidet, in welchem Menü dieser Eintrag angezeigt werden soll.")]
        public int Position { get; set; }

        [DisplayName("Titel des Menüeintrags, welcher im Menü erscheinen soll")]
        public string Title { get; set; }

        [DisplayName("Die Adresse auf die dieser Menüeintrag verweisen soll")]
        public string URL { get; set; }

        [DisplayName("Position im Menü")]
        public int MenuOrderID { get; set; }
    }
}