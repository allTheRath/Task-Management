namespace TaskAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NotificationUpdateId = c.Int(nullable: false),
                        Opened = c.Boolean(nullable: false),
                        notificationOptions = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Notifications");
        }
    }
}
