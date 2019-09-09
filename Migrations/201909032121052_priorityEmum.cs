namespace TaskAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class priorityEmum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "priority", c => c.Int(nullable: false));
            AddColumn("dbo.Tasks", "priority", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "priority");
            DropColumn("dbo.Projects", "priority");
        }
    }
}
