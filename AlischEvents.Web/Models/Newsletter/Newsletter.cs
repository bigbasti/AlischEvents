using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace AlischEvents.Web.Models.Newsletter
{
    public class Newsletter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Betreff")]
        public string Subject { get; set; }

        [Required]
        [AllowHtml]
        [DisplayName("Inhalt")]
        public string Content { get; set; }

        [Required]
        [DisplayName("Inhalt ist im HTML Format")]
        public bool HtmlContent { get; set; }

        [DisplayName("Newsletter wurde verschickt")]
        public bool WasSent { get; set; }

        public DateTime Date { get; set; }

        public int Recipients { get; set; }
    }
}