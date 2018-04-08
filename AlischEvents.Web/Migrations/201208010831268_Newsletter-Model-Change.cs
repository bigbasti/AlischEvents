namespace AlischEvents.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class NewsletterModelChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("NewsletterConsumers", "Newsletter_Id", "Newsletters");
            DropIndex("NewsletterConsumers", new[] { "Newsletter_Id" });
            AddColumn("Newsletters", "Recipients", c => c.Int(nullable: false));
            DropColumn("NewsletterConsumers", "Newsletter_Id");
        }
        
        public override void Down()
        {
            AddColumn("NewsletterConsumers", "Newsletter_Id", c => c.Int());
            DropColumn("Newsletters", "Recipients");
            CreateIndex("NewsletterConsumers", "Newsletter_Id");
            AddForeignKey("NewsletterConsumers", "Newsletter_Id", "Newsletters", "Id");
        }
    }
}
