namespace ProyectoFinal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class BreakCircularFKs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sessions", "HandId1", "dbo.Hands");
            DropForeignKey("dbo.Sessions", "HandId2", "dbo.Hands");
            DropForeignKey("dbo.Sessions", "HandId3", "dbo.Hands");
            DropForeignKey("dbo.Sessions", "HandId4", "dbo.Hands");
            DropIndex("dbo.Sessions", new[] { "HandId1" });
            DropIndex("dbo.Sessions", new[] { "HandId2" });
            DropIndex("dbo.Sessions", new[] { "HandId3" });
            DropIndex("dbo.Sessions", new[] { "HandId4" });
            AlterColumn("dbo.Sessions", "HandId1", c => c.Guid());
            AlterColumn("dbo.Sessions", "HandId2", c => c.Guid());
            AlterColumn("dbo.Sessions", "HandId3", c => c.Guid());
            AlterColumn("dbo.Sessions", "HandId4", c => c.Guid());
            CreateIndex("dbo.Sessions", "HandId1");
            CreateIndex("dbo.Sessions", "HandId2");
            CreateIndex("dbo.Sessions", "HandId3");
            CreateIndex("dbo.Sessions", "HandId4");
            AddForeignKey("dbo.Sessions", "HandId1", "dbo.Hands", "Id");
            AddForeignKey("dbo.Sessions", "HandId2", "dbo.Hands", "Id");
            AddForeignKey("dbo.Sessions", "HandId3", "dbo.Hands", "Id");
            AddForeignKey("dbo.Sessions", "HandId4", "dbo.Hands", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Sessions", "HandId4", "dbo.Hands");
            DropForeignKey("dbo.Sessions", "HandId3", "dbo.Hands");
            DropForeignKey("dbo.Sessions", "HandId2", "dbo.Hands");
            DropForeignKey("dbo.Sessions", "HandId1", "dbo.Hands");
            DropIndex("dbo.Sessions", new[] { "HandId4" });
            DropIndex("dbo.Sessions", new[] { "HandId3" });
            DropIndex("dbo.Sessions", new[] { "HandId2" });
            DropIndex("dbo.Sessions", new[] { "HandId1" });
            AlterColumn("dbo.Sessions", "HandId4", c => c.Guid(nullable: false));
            AlterColumn("dbo.Sessions", "HandId3", c => c.Guid(nullable: false));
            AlterColumn("dbo.Sessions", "HandId2", c => c.Guid(nullable: false));
            AlterColumn("dbo.Sessions", "HandId1", c => c.Guid(nullable: false));
            CreateIndex("dbo.Sessions", "HandId4");
            CreateIndex("dbo.Sessions", "HandId3");
            CreateIndex("dbo.Sessions", "HandId2");
            CreateIndex("dbo.Sessions", "HandId1");
            AddForeignKey("dbo.Sessions", "HandId4", "dbo.Hands", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Sessions", "HandId3", "dbo.Hands", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Sessions", "HandId2", "dbo.Hands", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Sessions", "HandId1", "dbo.Hands", "Id", cascadeDelete: false);
        }
    }
}
