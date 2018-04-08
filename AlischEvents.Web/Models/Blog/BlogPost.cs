using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace AlischEvents.Web.Models.Blog {
    public class BlogPost {

        [Key]
        public int PostID { get; set; }

        public int BlogID { get; set; }

        [DisplayName("Autor der Artikels")]
        public string Author { get; set; }

        [DisplayName("Datum")]
        public DateTime Date { get; set; }

        [DisplayName("Kommentare erlauben?")]
        public bool AllowComments { get; set; }

        [DisplayName("Vorschaugrafik")]
        public string PreviewImage { get; set; }

        [DisplayName("Veröffentlicht")]
        public bool  Published { get; set; }

        [Required]
        [DisplayName("Der Titel dieses Artikels")]
        public string Title { get; set; }

        [AllowHtml]
        [DisplayName("Kurzer Vorschau Text")]
        public string PreviewText { get; set; }

        [AllowHtml]
        [DisplayName("Artikelinhalt")]
        public string Content { get; set; }


        public virtual ICollection<PostComment> Comments { get; set; }
    }
}