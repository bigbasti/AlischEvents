using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AlischEvents.Web.Models.Galerie {
    public class AccessToken {

        [Key]
        public int TokenID { get; set; }

        [DisplayName("Gibt an, ob dieser Schlüssel noch benutzbar ist")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage="Bitte denken Sie sich einen Schlüssel aus")]
        [DisplayName("Der Schlüssel, den der Benutzer eingeben muss um Zugriff auf die Galerie oder Datei zu erhalten")]
        public string Token { get; set; }

        [DisplayName("Diesen Schlüssel auf eine bestimmte Datei einschränken. Format: \"galerie/dateiname\" Beispiel: \"hochzeit12/foto_012.jpg\"")]
        public string FileLimitation { get; set; }

        [DisplayName("Anzahl an Aufrufen nach der dieser Schlüssel unwirksam wird. (Für unbegrenzte Nutzung einen negatven Wert angeben. Beispiel: \"-1\"")]
        public int ClickLimitation { get; set; }

        [DisplayName("Das Datum ab dem dieser Schlüssel unwirksam wird.")]
        public DateTime DateLimitation { get; set; }
    }
}