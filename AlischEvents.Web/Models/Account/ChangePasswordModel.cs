using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel;

namespace AlischEvents.Web.Models.Account {
    public class ChangePasswordModel {

        [Required]
        public int AccountID { get; set; }

        [Required]
        [DisplayName("Neues Passwort")]
        public string Pass1 { get; set; }

        [Required, Compare("Pass1", ErrorMessage="Die beiden Passwörter stimmen nicht überein!")]
        [DisplayName("Neues Passwort wiederholen")]
        public string Pass2 { get; set; }
    }
}