namespace EbookObjects.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GutStandardUrls : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GutBook", "StandardEpubUrlNoImages", c => c.Boolean(nullable: false));
            AddColumn("dbo.GutBook", "StandardEpubUrlImages", c => c.Boolean(nullable: false));
            AddColumn("dbo.GutBook", "StandardThumbnailUrl", c => c.Boolean(nullable: false));
            AddColumn("dbo.GutBook", "StandardCoverUrl", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GutBook", "StandardCoverUrl");
            DropColumn("dbo.GutBook", "StandardThumbnailUrl");
            DropColumn("dbo.GutBook", "StandardEpubUrlImages");
            DropColumn("dbo.GutBook", "StandardEpubUrlNoImages");
        }
    }
}
