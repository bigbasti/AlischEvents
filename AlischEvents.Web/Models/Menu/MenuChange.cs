using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlischEvents.Web.Models.Menu {


    public class MenuChange {

        public int ChangeFrom { get; set; }             //Das Menüelement dass verschoben werden soll
        public int ChangeTo { get; set; }               //Das MEnüelement an dessen Stelle es rücken soll
        public string SiteTitle { get; set; }           //Der Titel der Seite die verschoebn werden soll
        public int SiteID { get; set; }
    }

}