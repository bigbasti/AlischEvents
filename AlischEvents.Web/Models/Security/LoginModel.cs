using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AlischEvents.Web.Models.Security {
    public class LoginModel {

        [Display(Name="Benutzername")]
        [Required(ErrorMessage = "Bitte geben Sie Ihren Benutzernamen ein")]
        public string Username { get; set; }

        [Display(Name = "Passwort")]
        [Required(ErrorMessage="Bitte geben Sie Ihr Passwort ein")]
        public string Password { get; set; }
    }
}