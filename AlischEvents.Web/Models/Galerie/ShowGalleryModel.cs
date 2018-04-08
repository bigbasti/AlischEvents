using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlischEvents.Web.Models.Galerie {
    public class ShowGalleryModel {

        public Gallery GalleryInfo { get; set; }

        public IEnumerable<string> ImagePaths { get; set; }

        public AccessToken Token { get; set; }
    }
}