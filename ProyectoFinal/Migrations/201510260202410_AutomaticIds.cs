namespace ProyectoFinal.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class AutomaticIds : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("dbo.HandCards", "Hand_Id", "dbo.Hands");
			DropForeignKey("dbo.Sessions", "HandId1", "dbo.Hands");
			DropForeignKey("dbo.Sessions", "HandId2", "dbo.Hands");
			DropForeignKey("dbo.Sessions", "HandId3", "dbo.Hands");
			DropForeignKey("dbo.Sessions", "HandId4", "dbo.Hands");
			DropForeignKey("dbo.CardInPiles", "SessionId", "dbo.Sessions");
			DropForeignKey("dbo.Hands", "SessionId", "dbo.Sessions");
			DropPrimaryKey("dbo.Hands");
			DropPrimaryKey("dbo.Sessions");
			AlterColumn("dbo.Hands", "Id", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newid()"));
			AlterColumn("dbo.Sessions", "Id", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newid()"));
			AddPrimaryKey("dbo.Hands", "Id");
			AddPrimaryKey("dbo.Sessions", "Id");
			AddForeignKey("dbo.HandCards", "Hand_Id", "dbo.Hands", "Id", cascadeDelete: false);
			AddForeignKey("dbo.Sessions", "HandId1", "dbo.Hands", "Id");
			AddForeignKey("dbo.Sessions", "HandId2", "dbo.Hands", "Id");
			AddForeignKey("dbo.Sessions", "HandId3", "dbo.Hands", "Id");
			AddForeignKey("dbo.Sessions", "HandId4", "dbo.Hands", "Id");
			AddForeignKey("dbo.CardInPiles", "SessionId", "dbo.Sessions", "Id", cascadeDelete: false);
			AddForeignKey("dbo.Hands", "SessionId", "dbo.Sessions", "Id", cascadeDelete: false);
		}

		public override void Down()
		{
			DropForeignKey("dbo.Hands", "SessionId", "dbo.Sessions");
			DropForeignKey("dbo.CardInPiles", "SessionId", "dbo.Sessions");
			DropForeignKey("dbo.Sessions", "HandId4", "dbo.Hands");
			DropForeignKey("dbo.Sessions", "HandId3", "dbo.Hands");
			DropForeignKey("dbo.Sessions", "HandId2", "dbo.Hands");
			DropForeignKey("dbo.Sessions", "HandId1", "dbo.Hands");
			DropForeignKey("dbo.HandCards", "Hand_Id", "dbo.Hands");
			DropPrimaryKey("dbo.Sessions");
			DropPrimaryKey("dbo.Hands");
			AlterColumn("dbo.Sessions", "Id", c => c.Guid(nullable: false));
			AlterColumn("dbo.Hands", "Id", c => c.Guid(nullable: false));
			AddPrimaryKey("dbo.Sessions", "Id");
			AddPrimaryKey("dbo.Hands", "Id");
			AddForeignKey("dbo.Hands", "SessionId", "dbo.Sessions", "Id", cascadeDelete: false);
			AddForeignKey("dbo.CardInPiles", "SessionId", "dbo.Sessions", "Id", cascadeDelete: false);
			AddForeignKey("dbo.Sessions", "HandId4", "dbo.Hands", "Id");
			AddForeignKey("dbo.Sessions", "HandId3", "dbo.Hands", "Id");
			AddForeignKey("dbo.Sessions", "HandId2", "dbo.Hands", "Id");
			AddForeignKey("dbo.Sessions", "HandId1", "dbo.Hands", "Id");
			AddForeignKey("dbo.HandCards", "Hand_Id", "dbo.Hands", "Id", cascadeDelete: false);
		}
	}
}
