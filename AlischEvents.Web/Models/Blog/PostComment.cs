using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AlischEvents.Web.Models.Blog {
    public class PostComment {

        [Key]
        public int CommentID { get; set; }

        public int PostID { get; set; }

        public string Author { get; set; }

        public DateTime Date { get; set; }

        public string Content { get; set; }

        public string Email { get; set; }
    }
}