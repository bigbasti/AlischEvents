using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AlischEvents.Web.Models.Site
{
    /// <summary>
    /// Modelklasse für die Datenbankstruktur
    /// ACHTUNG: Jede Änderung hat zur Folge, dass die Datenbank neu generiert wird und alle Daten Verloren gehen!
    /// </summary>
    public class WebSite
    {
        [Key]
        public int SiteID { get; set; }

        [Required]
        [DisplayName("Titel der Seite")]
        public string SiteTitle { get; set; }

        [Required]
        [DisplayName("Bezeichnung der Seite")]
        public string SiteLabel { get; set; }

        [AllowHtml]
        [DisplayName("Seiteninhalt")]
        public string SiteContent { get; set; }

        [DisplayName("Im Menü anzeigen?")]
        public bool ShowOnMenu { get; set; }

        [DisplayName("Position im Menü")]
        public int MenuOrderID { get; set; }

        [DisplayName("Besondere Seite?")]
        public bool IsSpecialSite { get; set; }

        [DisplayName("Statische Seite?")]
        public bool IsStaticSite { get; set; }

        [DisplayName("Adresse der Seite")]
        public string SiteURL { get; set; }
    }
}
