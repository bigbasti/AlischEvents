using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlischEvents.Web.Models.Admin {
    public class UserOverviewModel {

        public IQueryable<SiteUser> Users { get; set; }
    }
}