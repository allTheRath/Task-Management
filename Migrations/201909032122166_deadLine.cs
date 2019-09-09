namespace TaskAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deadLine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "DeadLine", c => c.DateTime(nullable: false));
            AddColumn("dbo.Tasks", "DeadLine", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "DeadLine");
            DropColumn("dbo.Projects", "DeadLine");
        }
    }
}
