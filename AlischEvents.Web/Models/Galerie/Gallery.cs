using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace AlischEvents.Web.Models.Galerie {
    public class Gallery {

        [Key]
        public int GalleryID { get; set; }

        [Required]
        [DisplayName("Der Titel der Galerie so wie der Benutzer diesen sehen soll")]
        public string Title { get; set; }

        [Required]
        [Remote("FolderAvailable", "Gallery", ErrorMessage="Dieser Ordnername wird bereits von einer anderen Galerie genutzt!")]
        [DisplayName("Der Name des Ordners, in dem diese Galerie gespeicher ist oder werden soll")]
        public string FolderName { get; set; }

        /// <summary>
        /// Die Zugangsschlüssel mit denen man Zugriff auf diese Galerie erhält
        /// </summary>
        public virtual LinkedList<AccessToken> AccessTokens { get; set; }
    }
}