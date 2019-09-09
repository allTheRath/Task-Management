namespace TaskAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedTaskToProject : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tasks", "Project_Id", "dbo.Projects");
            DropIndex("dbo.Tasks", new[] { "Project_Id" });
            RenameColumn(table: "dbo.Tasks", name: "Project_Id", newName: "ProjectId");
            AlterColumn("dbo.Tasks", "ProjectId", c => c.Int(nullable: false));
            CreateIndex("dbo.Tasks", "ProjectId");
            AddForeignKey("dbo.Tasks", "ProjectId", "dbo.Projects", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "ProjectId", "dbo.Projects");
            DropIndex("dbo.Tasks", new[] { "ProjectId" });
            AlterColumn("dbo.Tasks", "ProjectId", c => c.Int());
            RenameColumn(table: "dbo.Tasks", name: "ProjectId", newName: "Project_Id");
            CreateIndex("dbo.Tasks", "Project_Id");
            AddForeignKey("dbo.Tasks", "Project_Id", "dbo.Projects", "Id");
        }
    }
}
