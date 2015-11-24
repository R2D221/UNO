namespace ProyectoFinal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastPlayed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "LastPlayed", c => c.DateTime(nullable: false, defaultValue: DateTime.UtcNow));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "LastPlayed");
        }
    }
}
