namespace TaskAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedNotesTask : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "notes");
        }
    }
}
