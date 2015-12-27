namespace EbookObjects.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Languages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LanguageCode",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 3),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "dbo.LanguageName",
                c => new
                    {
                        LanguageNameId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.LanguageNameId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.LanguageNameLanguageCodes",
                c => new
                    {
                        LanguageName_LanguageNameId = c.Int(nullable: false),
                        LanguageCode_Code = c.String(nullable: false, maxLength: 3),
                    })
                .PrimaryKey(t => new { t.LanguageName_LanguageNameId, t.LanguageCode_Code })
                .ForeignKey("dbo.LanguageName", t => t.LanguageName_LanguageNameId, cascadeDelete: true)
                .ForeignKey("dbo.LanguageCode", t => t.LanguageCode_Code, cascadeDelete: true)
                .Index(t => t.LanguageName_LanguageNameId)
                .Index(t => t.LanguageCode_Code);
            
            AlterColumn("dbo.GutBook", "Language", c => c.String(maxLength: 3));
            CreateIndex("dbo.GutBook", "Language");
            AddForeignKey("dbo.GutBook", "Language", "dbo.LanguageCode", "Code");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GutBook", "Language", "dbo.LanguageCode");
            DropForeignKey("dbo.LanguageNameLanguageCodes", "LanguageCode_Code", "dbo.LanguageCode");
            DropForeignKey("dbo.LanguageNameLanguageCodes", "LanguageName_LanguageNameId", "dbo.LanguageName");
            DropIndex("dbo.LanguageNameLanguageCodes", new[] { "LanguageCode_Code" });
            DropIndex("dbo.LanguageNameLanguageCodes", new[] { "LanguageName_LanguageNameId" });
            DropIndex("dbo.LanguageName", new[] { "Name" });
            DropIndex("dbo.GutBook", new[] { "Language" });
            AlterColumn("dbo.GutBook", "Language", c => c.String(maxLength: 5));
            DropTable("dbo.LanguageNameLanguageCodes");
            DropTable("dbo.LanguageName");
            DropTable("dbo.LanguageCode");
        }
    }
}
