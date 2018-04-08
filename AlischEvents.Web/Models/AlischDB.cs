using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using AlischEvents.Web.Models.Admin;
using AlischEvents.Web.Models.Site;
using AlischEvents.Web.Models.Blog;
using AlischEvents.Web.Models.Galerie;

namespace AlischEvents.Web.Models {
    public class AlischDB : DbContext {

        public AlischDB()
            : base("AlischDB") {

        }

        public DbSet<SiteUser> SiteUsers { get; set; }
        public DbSet<WebSite> WebSites { get; set; }
        public DbSet<Blog.Blog> Blogs { get; set; }
        public DbSet<Blog.BlogPost> BlogsPosts { get; set; }
        public DbSet<Blog.PostComment> BlogPostComments { get; set; }
        public DbSet<Menu.Menu> BlogMenus { get; set; }
        public DbSet<Menu.MenuEntry> MenuEntries { get; set; }
        public DbSet<Kontakt.ContactRequest> ContactRequests { get; set; }
        public DbSet<Galerie.Gallery> Gallerys { get; set; }
        public DbSet<Galerie.AccessToken> AccessTokens { get; set; }
        public DbSet<Newsletter.Newsletter> Newsletters { get; set; }
        public DbSet<Newsletter.NewsletterConsumer> NewsletterConsumers { get; set; }
    }
}