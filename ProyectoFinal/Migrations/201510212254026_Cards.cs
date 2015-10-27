namespace ProyectoFinal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cards : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Color = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Hands",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        SessionId = c.Guid(nullable: false),
                        IsTheirTurn = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sessions", t => t.SessionId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.SessionId);
            
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DiscardPileTopId = c.Int(nullable: false),
                        Direction = c.Int(nullable: false),
                        HandId1 = c.Guid(nullable: false),
                        HandId2 = c.Guid(nullable: false),
                        HandId3 = c.Guid(nullable: false),
                        HandId4 = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cards", t => t.DiscardPileTopId, cascadeDelete: false)
                .ForeignKey("dbo.Hands", t => t.HandId1, cascadeDelete: false)
                .ForeignKey("dbo.Hands", t => t.HandId2, cascadeDelete: false)
                .ForeignKey("dbo.Hands", t => t.HandId3, cascadeDelete: false)
                .ForeignKey("dbo.Hands", t => t.HandId4, cascadeDelete: false)
                .Index(t => t.DiscardPileTopId)
                .Index(t => t.HandId1)
                .Index(t => t.HandId2)
                .Index(t => t.HandId3)
                .Index(t => t.HandId4);
            
            CreateTable(
                "dbo.CardInPiles",
                c => new
                    {
                        SessionId = c.Guid(nullable: false),
                        CardId = c.Int(nullable: false),
                        IsDiscarded = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SessionId, t.CardId })
                .ForeignKey("dbo.Cards", t => t.CardId, cascadeDelete: false)
                .ForeignKey("dbo.Sessions", t => t.SessionId, cascadeDelete: false)
                .Index(t => t.SessionId)
                .Index(t => t.CardId);
            
            CreateTable(
                "dbo.HandCards",
                c => new
                    {
                        Hand_Id = c.Guid(nullable: false),
                        Card_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Hand_Id, t.Card_Id })
                .ForeignKey("dbo.Hands", t => t.Hand_Id, cascadeDelete: false)
                .ForeignKey("dbo.Cards", t => t.Card_Id, cascadeDelete: false)
                .Index(t => t.Hand_Id)
                .Index(t => t.Card_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hands", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Hands", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.Sessions", "HandId4", "dbo.Hands");
            DropForeignKey("dbo.Sessions", "HandId3", "dbo.Hands");
            DropForeignKey("dbo.Sessions", "HandId2", "dbo.Hands");
            DropForeignKey("dbo.Sessions", "HandId1", "dbo.Hands");
            DropForeignKey("dbo.Sessions", "DiscardPileTopId", "dbo.Cards");
            DropForeignKey("dbo.CardInPiles", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.CardInPiles", "CardId", "dbo.Cards");
            DropForeignKey("dbo.HandCards", "Card_Id", "dbo.Cards");
            DropForeignKey("dbo.HandCards", "Hand_Id", "dbo.Hands");
            DropIndex("dbo.HandCards", new[] { "Card_Id" });
            DropIndex("dbo.HandCards", new[] { "Hand_Id" });
            DropIndex("dbo.CardInPiles", new[] { "CardId" });
            DropIndex("dbo.CardInPiles", new[] { "SessionId" });
            DropIndex("dbo.Sessions", new[] { "HandId4" });
            DropIndex("dbo.Sessions", new[] { "HandId3" });
            DropIndex("dbo.Sessions", new[] { "HandId2" });
            DropIndex("dbo.Sessions", new[] { "HandId1" });
            DropIndex("dbo.Sessions", new[] { "DiscardPileTopId" });
            DropIndex("dbo.Hands", new[] { "SessionId" });
            DropIndex("dbo.Hands", new[] { "UserId" });
            DropTable("dbo.HandCards");
            DropTable("dbo.CardInPiles");
            DropTable("dbo.Sessions");
            DropTable("dbo.Hands");
            DropTable("dbo.Cards");
        }
    }
}
