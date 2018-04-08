using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AlischEvents.Web.Models.Blog {
    public class Blog  {

        [Key]
        public int BlogID { get; set; }

        [DisplayName("Position im Menü")]
        public int MenuOrderID { get; set; }

        [DisplayName("Name des Blogs")]
        public string Name { get; set; }

        [DisplayName("Titel des Blogs")]
        public string Title { get; set; }

        public virtual ICollection<BlogPost> Posts { get; set; }
    }
}