using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AlischEvents.Web.Models.Account {
    public class RestorePasswordModel {

        [Required]
        [DisplayName("Ihre Email-Adresse")]
        public string Email { get; set; }
    }
}