namespace EbookObjects.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CoverContentType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cover", "ContentType", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cover", "ContentType");
        }
    }
}
