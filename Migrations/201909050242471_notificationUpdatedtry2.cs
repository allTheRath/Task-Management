namespace TaskAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationUpdatedtry2 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.NotificationViewModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.NotificationViewModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Opened = c.Boolean(nullable: false),
                        Title = c.String(),
                        Details = c.String(),
                        priority = c.Int(nullable: false),
                        DeadLine = c.DateTime(nullable: false),
                        notificationOptions = c.String(),
                        NotificationUpdateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
