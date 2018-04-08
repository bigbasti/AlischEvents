namespace AlischEvents.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Newsletter : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Newsletters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(nullable: false),
                        Content = c.String(nullable: false),
                        HtmlContent = c.Boolean(nullable: false),
                        WasSent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "NewsletterConsumers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        EmailsReceived = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Newsletter_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Newsletters", t => t.Newsletter_Id)
                .Index(t => t.Newsletter_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("NewsletterConsumers", new[] { "Newsletter_Id" });
            DropForeignKey("NewsletterConsumers", "Newsletter_Id", "Newsletters");
            DropTable("NewsletterConsumers");
            DropTable("Newsletters");
        }
    }
}
