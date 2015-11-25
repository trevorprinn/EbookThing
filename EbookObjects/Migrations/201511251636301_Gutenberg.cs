namespace EbookObjects.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Gutenberg : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GutBook",
                c => new
                    {
                        GutBookId = c.Int(nullable: false),
                        GutAuthorId = c.Int(),
                        Title = c.String(),
                        Language = c.String(maxLength: 5),
                        EpubUrlNoImages = c.String(),
                        EpubUrlImages = c.String(),
                        ThumbnailUrl = c.String(),
                        ThumbnailData = c.Binary(),
                        CoverUrl = c.String(),
                    })
                .PrimaryKey(t => t.GutBookId)
                .ForeignKey("dbo.GutAuthor", t => t.GutAuthorId)
                .Index(t => t.GutAuthorId);
            
            CreateTable(
                "dbo.GutAuthor",
                c => new
                    {
                        GutAuthorId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.GutAuthorId)
                .Index(t => t.Name, unique: true, name: "IX_GutAuthor_Name");
            
            AddColumn("dbo.EpubFile", "GutBookId", c => c.Int());
            AddColumn("dbo.EpubFile", "GutBookWithImages", c => c.Boolean());
            CreateIndex("dbo.EpubFile", "GutBookId");
            AddForeignKey("dbo.EpubFile", "GutBookId", "dbo.GutBook", "GutBookId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GutBook", "GutAuthorId", "dbo.GutAuthor");
            DropForeignKey("dbo.EpubFile", "GutBookId", "dbo.GutBook");
            DropIndex("dbo.GutAuthor", "IX_GutAuthor_Name");
            DropIndex("dbo.GutBook", new[] { "GutAuthorId" });
            DropIndex("dbo.EpubFile", new[] { "GutBookId" });
            DropColumn("dbo.EpubFile", "GutBookWithImages");
            DropColumn("dbo.EpubFile", "GutBookId");
            DropTable("dbo.GutAuthor");
            DropTable("dbo.GutBook");
        }
    }
}
