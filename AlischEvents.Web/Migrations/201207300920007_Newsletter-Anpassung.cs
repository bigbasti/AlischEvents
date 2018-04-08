namespace AlischEvents.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class NewsletterAnpassung : DbMigration
    {
        public override void Up()
        {
            AddColumn("Newsletters", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Newsletters", "Date");
        }
    }
}
