using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AlischEvents.Web.Models.Kontakt {
    public class ContactRequest {
        
        [Key]
        public int RequestID { get; set; }

        public DateTime Date { get; set; }

        public bool Read { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }
    }
}