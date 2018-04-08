using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AlischEvents.Web.Models.Newsletter
{
    public class NewsletterConsumer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("E-Mail Adresse")]
        public string Email { get; set; }

        /// <summary>
        /// Speichert die Anzahl an Newslettern, die der Nutzer empfangen hat
        /// Ist dieser Wert negativ, bedeutet es, dass der Benutzer den Bestätigungslink noch nicht geklickt hat
        /// </summary>
        public int EmailsReceived { get; set; }


        /// <summary>
        /// Gibt an ob dieser user weitere Newsletter haben möchte
        /// Wenn False gillt das als Abmeldung vom Newsletter
        /// Bei erneuter Registrierung wird dieser Flag wieder auf true gesetzt
        /// </summary>
        public bool Active { get; set; }
    }
}