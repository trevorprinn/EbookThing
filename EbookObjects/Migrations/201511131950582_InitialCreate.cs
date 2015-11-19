namespace EbookObjects.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRole",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserRole",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                        IdentityRole_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
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
                        UserId = c.String(),
                        ClaimType = c.String(maxLength: 150),
                        ClaimValue = c.String(maxLength: 500),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.AspNetUserLogin",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUser", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.AspNetUser",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(maxLength: 500),
                        SecurityStamp = c.String(maxLength: 500),
                        PhoneNumber = c.String(maxLength: 50),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Author",
                c => new
                    {
                        AuthorId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AuthorId);
            
            CreateTable(
                "dbo.Book",
                c => new
                    {
                        BookId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        AuthorId = c.Int(),
                        PublisherId = c.Int(),
                        CoverId = c.Int(),
                        FileId = c.Int(nullable: false),
                        SeriesId = c.Int(),
                        SeriesNbr = c.Decimal(precision: 18, scale: 2),
                        Description = c.String(),
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
                        Identifier = c.String(nullable: false, maxLength: 150),
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
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.IdentId);
            
            CreateTable(
                "dbo.Cover",
                c => new
                    {
                        CoverId = c.Int(nullable: false, identity: true),
                        Picture = c.Binary(nullable: false),
                        Thumbnail = c.Binary(nullable: false),
                        Checksum = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.CoverId);
            
            CreateTable(
                "dbo.EpubFile",
                c => new
                    {
                        FileId = c.Int(nullable: false, identity: true),
                        Contents = c.Binary(nullable: false),
                        Checksum = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.FileId);
            
            CreateTable(
                "dbo.Publisher",
                c => new
                    {
                        PublisherId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.PublisherId);
            
            CreateTable(
                "dbo.Series",
                c => new
                    {
                        SeriesId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.SeriesId);
            
            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        Item = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.TagId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Identity = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.Identity, unique: true, name: "IX_UserIdentity");
            
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
            DropIndex("dbo.User", "IX_UserIdentity");
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
            DropTable("dbo.User");
            DropTable("dbo.Tag");
            DropTable("dbo.Series");
            DropTable("dbo.Publisher");
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
