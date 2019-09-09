namespace TaskAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usertypes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "userType", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "userType", c => c.Int());
        }
    }
}
