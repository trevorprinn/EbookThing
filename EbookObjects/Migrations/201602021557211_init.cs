namespace EbookObjects.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRole",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Name = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserRole",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        RoleId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        IdentityRole_Id = c.String(maxLength: 128, storeType: "nvarchar"),
                        ApplicationUser_Id = c.String(maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRole", t => t.IdentityRole_Id)
                .ForeignKey("dbo.AspNetUser", t => t.ApplicationUser_Id)
                .Index(t => t.IdentityRole_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.AspNetUserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(unicode: false),
                        ClaimType = c.String(maxLength: 150, storeType: "nvarchar"),
                        ClaimValue = c.String(maxLength: 500, storeType: "nvarchar"),
                        ApplicationUser_Id = c.String(maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.AspNetUserLogin",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ProviderKey = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ApplicationUser_Id = c.String(maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.AspNetUser",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Email = c.String(unicode: false),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(maxLength: 500, storeType: "nvarchar"),
                        SecurityStamp = c.String(maxLength: 500, storeType: "nvarchar"),
                        PhoneNumber = c.String(maxLength: 50, storeType: "nvarchar"),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(precision: 0),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Author",
                c => new
                    {
                        AuthorId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.AuthorId);
            
            CreateTable(
                "dbo.Book",
                c => new
                    {
                        BookId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Title = c.String(nullable: false, unicode: false),
                        AuthorId = c.Int(),
                        PublisherId = c.Int(),
                        CoverId = c.Int(),
                        FileId = c.Int(nullable: false),
                        SeriesId = c.Int(),
                        SeriesNbr = c.Decimal(precision: 18, scale: 2),
                        Description = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.BookId)
                .ForeignKey("dbo.Author", t => t.AuthorId)
                .ForeignKey("dbo.Cover", t => t.CoverId)
                .ForeignKey("dbo.EpubFile", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.Publisher", t => t.PublisherId)
                .ForeignKey("dbo.Series", t => t.SeriesId)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.AuthorId)
                .Index(t => t.PublisherId)
                .Index(t => t.CoverId)
                .Index(t => t.FileId)
                .Index(t => t.SeriesId);
            
            CreateTable(
                "dbo.BookIdent",
                c => new
                    {
                        BookId = c.Int(nullable: false),
                        IdentId = c.Int(nullable: false),
                        Identifier = c.String(nullable: false, maxLength: 150, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.BookId, t.IdentId })
                .ForeignKey("dbo.Book", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Ident", t => t.IdentId, cascadeDelete: true)
                .Index(t => t.BookId)
                .Index(t => t.IdentId);
            
            CreateTable(
                "dbo.Ident",
                c => new
                    {
                        IdentId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.IdentId);
            
            CreateTable(
                "dbo.Cover",
                c => new
                    {
                        CoverId = c.Int(nullable: false, identity: true),
                        Picture = c.Binary(nullable: false),
                        Thumbnail = c.Binary(nullable: false),
                        Checksum = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        ContentType = c.String(maxLength: 50, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.CoverId);
            
            CreateTable(
                "dbo.EpubFile",
                c => new
                    {
                        FileId = c.Int(nullable: false, identity: true),
                        Contents = c.Binary(nullable: false),
                        Checksum = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        GutBookId = c.Int(),
                        GutBookWithImages = c.Boolean(),
                    })
                .PrimaryKey(t => t.FileId)
                .ForeignKey("dbo.GutBook", t => t.GutBookId)
                .Index(t => t.GutBookId);
            
            CreateTable(
                "dbo.GutBook",
                c => new
                    {
                        GutBookId = c.Int(nullable: false),
                        GutAuthorId = c.Int(),
                        Title = c.String(unicode: false),
                        Language = c.String(maxLength: 3, storeType: "nvarchar"),
                        StandardEpubUrlNoImages = c.Boolean(nullable: false),
                        StandardEpubUrlImages = c.Boolean(nullable: false),
                        EpubUrlNoImages = c.String(unicode: false),
                        EpubUrlImages = c.String(unicode: false),
                        StandardThumbnailUrl = c.Boolean(nullable: false),
                        StandardCoverUrl = c.Boolean(nullable: false),
                        ThumbnailUrl = c.String(unicode: false),
                        ThumbnailData = c.Binary(),
                        CoverUrl = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.GutBookId)
                .ForeignKey("dbo.GutAuthor", t => t.GutAuthorId)
                .ForeignKey("dbo.LanguageCode", t => t.Language)
                .Index(t => t.GutAuthorId)
                .Index(t => t.Language);
            
            CreateTable(
                "dbo.GutAuthor",
                c => new
                    {
                        GutAuthorId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.GutAuthorId)
                .Index(t => t.Name, unique: true, name: "IX_GutAuthor_Name");
            
            CreateTable(
                "dbo.LanguageCode",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 3, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "dbo.LanguageName",
                c => new
                    {
                        LanguageNameId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.LanguageNameId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Publisher",
                c => new
                    {
                        PublisherId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.PublisherId);
            
            CreateTable(
                "dbo.Series",
                c => new
                    {
                        SeriesId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.SeriesId);
            
            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        Item = c.String(nullable: false, maxLength: 250, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.TagId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100, storeType: "nvarchar"),
                        Identity = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.Identity, unique: true, name: "IX_UserIdentity");
            
            CreateTable(
                "dbo.LanguageNameLanguageCodes",
                c => new
                    {
                        LanguageName_LanguageNameId = c.Int(nullable: false),
                        LanguageCode_Code = c.String(nullable: false, maxLength: 3, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.LanguageName_LanguageNameId, t.LanguageCode_Code })
                .ForeignKey("dbo.LanguageName", t => t.LanguageName_LanguageNameId, cascadeDelete: true)
                .ForeignKey("dbo.LanguageCode", t => t.LanguageCode_Code, cascadeDelete: true)
                .Index(t => t.LanguageName_LanguageNameId)
                .Index(t => t.LanguageCode_Code);
            
            CreateTable(
                "dbo.BookTag",
                c => new
                    {
                        BookId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BookId, t.TagId })
                .ForeignKey("dbo.Book", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.Tag", t => t.TagId, cascadeDelete: true)
                .Index(t => t.BookId)
                .Index(t => t.TagId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Book", "UserId", "dbo.User");
            DropForeignKey("dbo.BookTag", "TagId", "dbo.Tag");
            DropForeignKey("dbo.BookTag", "BookId", "dbo.Book");
            DropForeignKey("dbo.Book", "SeriesId", "dbo.Series");
            DropForeignKey("dbo.Book", "PublisherId", "dbo.Publisher");
            DropForeignKey("dbo.Book", "FileId", "dbo.EpubFile");
            DropForeignKey("dbo.GutBook", "Language", "dbo.LanguageCode");
            DropForeignKey("dbo.LanguageNameLanguageCodes", "LanguageCode_Code", "dbo.LanguageCode");
            DropForeignKey("dbo.LanguageNameLanguageCodes", "LanguageName_LanguageNameId", "dbo.LanguageName");
            DropForeignKey("dbo.GutBook", "GutAuthorId", "dbo.GutAuthor");
            DropForeignKey("dbo.EpubFile", "GutBookId", "dbo.GutBook");
            DropForeignKey("dbo.Book", "CoverId", "dbo.Cover");
            DropForeignKey("dbo.BookIdent", "IdentId", "dbo.Ident");
            DropForeignKey("dbo.BookIdent", "BookId", "dbo.Book");
            DropForeignKey("dbo.Book", "AuthorId", "dbo.Author");
            DropForeignKey("dbo.AspNetUserRole", "ApplicationUser_Id", "dbo.AspNetUser");
            DropForeignKey("dbo.AspNetUserLogin", "ApplicationUser_Id", "dbo.AspNetUser");
            DropForeignKey("dbo.AspNetUserClaim", "ApplicationUser_Id", "dbo.AspNetUser");
            DropForeignKey("dbo.AspNetUserRole", "IdentityRole_Id", "dbo.AspNetRole");
            DropIndex("dbo.BookTag", new[] { "TagId" });
            DropIndex("dbo.BookTag", new[] { "BookId" });
            DropIndex("dbo.LanguageNameLanguageCodes", new[] { "LanguageCode_Code" });
            DropIndex("dbo.LanguageNameLanguageCodes", new[] { "LanguageName_LanguageNameId" });
            DropIndex("dbo.User", "IX_UserIdentity");
            DropIndex("dbo.LanguageName", new[] { "Name" });
            DropIndex("dbo.GutAuthor", "IX_GutAuthor_Name");
            DropIndex("dbo.GutBook", new[] { "Language" });
            DropIndex("dbo.GutBook", new[] { "GutAuthorId" });
            DropIndex("dbo.EpubFile", new[] { "GutBookId" });
            DropIndex("dbo.BookIdent", new[] { "IdentId" });
            DropIndex("dbo.BookIdent", new[] { "BookId" });
            DropIndex("dbo.Book", new[] { "SeriesId" });
            DropIndex("dbo.Book", new[] { "FileId" });
            DropIndex("dbo.Book", new[] { "CoverId" });
            DropIndex("dbo.Book", new[] { "PublisherId" });
            DropIndex("dbo.Book", new[] { "AuthorId" });
            DropIndex("dbo.Book", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogin", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserClaim", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserRole", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserRole", new[] { "IdentityRole_Id" });
            DropTable("dbo.BookTag");
            DropTable("dbo.LanguageNameLanguageCodes");
            DropTable("dbo.User");
            DropTable("dbo.Tag");
            DropTable("dbo.Series");
            DropTable("dbo.Publisher");
            DropTable("dbo.LanguageName");
            DropTable("dbo.LanguageCode");
            DropTable("dbo.GutAuthor");
            DropTable("dbo.GutBook");
            DropTable("dbo.EpubFile");
            DropTable("dbo.Cover");
            DropTable("dbo.Ident");
            DropTable("dbo.BookIdent");
            DropTable("dbo.Book");
            DropTable("dbo.Author");
            DropTable("dbo.AspNetUser");
            DropTable("dbo.AspNetUserLogin");
            DropTable("dbo.AspNetUserClaim");
            DropTable("dbo.AspNetUserRole");
            DropTable("dbo.AspNetRole");
        }
    }
}
