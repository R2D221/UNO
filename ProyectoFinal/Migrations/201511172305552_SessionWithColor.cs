namespace ProyectoFinal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SessionWithColor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "Color", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "Color");
        }
    }
}
