using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AlischEvents.Web.Models.Menu {
    public class Menu {

        [Key]
        public int MenuID { get; set; }

        public virtual LinkedList<MenuEntry> MenuEntries { get; set; }
    }
}