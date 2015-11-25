namespace EbookObjects.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Gutenberg : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GutBooks",
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
                        GutAuthor_GutAuthorId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.GutBookId)
                .ForeignKey("dbo.GutAuthors", t => t.GutAuthor_GutAuthorId)
                .Index(t => t.GutAuthor_GutAuthorId);
            
            CreateTable(
                "dbo.GutAuthors",
                c => new
                    {
                        GutAuthorId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.GutAuthorId)
                .Index(t => t.Name, unique: true, name: "IX_GutAuthor_Name");
            
            AddColumn("dbo.EpubFile", "GutBookId", c => c.Int());
            AddColumn("dbo.EpubFile", "GutBookWithImages", c => c.Boolean());
            CreateIndex("dbo.EpubFile", "GutBookId");
            AddForeignKey("dbo.EpubFile", "GutBookId", "dbo.GutBooks", "GutBookId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GutBooks", "GutAuthor_GutAuthorId", "dbo.GutAuthors");
            DropForeignKey("dbo.EpubFile", "GutBookId", "dbo.GutBooks");
            DropIndex("dbo.GutAuthors", "IX_GutAuthor_Name");
            DropIndex("dbo.GutBooks", new[] { "GutAuthor_GutAuthorId" });
            DropIndex("dbo.EpubFile", new[] { "GutBookId" });
            DropColumn("dbo.EpubFile", "GutBookWithImages");
            DropColumn("dbo.EpubFile", "GutBookId");
            DropTable("dbo.GutAuthors");
            DropTable("dbo.GutBooks");
        }
    }
}
