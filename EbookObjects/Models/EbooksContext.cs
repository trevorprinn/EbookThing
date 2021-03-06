using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EbookObjects.Models.Mapping;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using MySql.Data.Entity;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public partial class EbooksContext : DbContext {
        static EbooksContext() {
            Database.SetInitializer<EbooksContext>(new MigrateDatabaseToLatestVersion<EbooksContext, Migrations.Configuration>());
        }

        private static bool _langSetup = false;

        public EbooksContext()
            : base("Name=EbooksContext") {
            if (!_langSetup) {
                try {
                    if (!LanguageCodes.Any()) {
                        initLanguageTable();
                    }
                    _langSetup = true;
                } catch { }
            }
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookIdent> BookIdents { get; set; }
        public DbSet<Cover> Covers { get; set; }
        public DbSet<EpubFile> EpubFiles { get; set; }
        public DbSet<Ident> Idents { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        public virtual DbSet<GutBook> GutBooks { get; set; }
        public virtual DbSet<GutAuthor> GutAuthors { get; set; }

        public virtual DbSet<LanguageCode> LanguageCodes { get; set; }
        public virtual DbSet<LanguageName> LanguageNames { get; set; }

        //
        // These are required for the integrated user membership.
        //
        public virtual DbSet<IdentityRole> AspNetRoles { get; set; }
        public virtual DbSet<ApplicationUser> AspNetUsers { get; set; }
        public virtual DbSet<IdentityUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<IdentityUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<IdentityUserRole> AspNetUserRoles { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Configurations.Add(new AuthorMap());
            modelBuilder.Configurations.Add(new BookMap());
            modelBuilder.Configurations.Add(new BookIdentMap());
            modelBuilder.Configurations.Add(new CoverMap());
            modelBuilder.Configurations.Add(new EpubFileMap());
            modelBuilder.Configurations.Add(new IdentMap());
            modelBuilder.Configurations.Add(new PublisherMap());
            modelBuilder.Configurations.Add(new SeriesMap());
            modelBuilder.Configurations.Add(new TagMap());
            modelBuilder.Configurations.Add(new UserMap());

            modelBuilder.Entity<User>()
                .HasIndex("IX_UserIdentity",
                    IndexOptions.Unique,
                    e => e.Property(x => x.Identity));

            // Configure Asp Net Identity Tables
            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUser");
            modelBuilder.Entity<ApplicationUser>().HasKey(u => u.Id);
            modelBuilder.Entity<ApplicationUser>().Property(u => u.PasswordHash).HasMaxLength(500);
            modelBuilder.Entity<ApplicationUser>().Property(u => u.SecurityStamp).HasMaxLength(500);
            modelBuilder.Entity<ApplicationUser>().Property(u => u.PhoneNumber).HasMaxLength(50);

            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRole");
            modelBuilder.Entity<IdentityRole>().HasKey(u => u.Id);

            modelBuilder.Entity<IdentityUserRole>().ToTable("AspNetUserRole");
            modelBuilder.Entity<IdentityUserRole>().HasKey(u => new { u.UserId, u.RoleId });

            modelBuilder.Entity<IdentityUserLogin>().ToTable("AspNetUserLogin");
            modelBuilder.Entity<IdentityUserLogin>().HasKey(u => new { u.LoginProvider, u.ProviderKey, u.UserId });

            modelBuilder.Entity<IdentityUserClaim>().ToTable("AspNetUserClaim");
            modelBuilder.Entity<IdentityUserClaim>().HasKey(u => u.Id);
            modelBuilder.Entity<IdentityUserClaim>().Property(u => u.ClaimType).HasMaxLength(150);
            modelBuilder.Entity<IdentityUserClaim>().Property(u => u.ClaimValue).HasMaxLength(500);
        }

        public static EbooksContext Create() {
            return new EbooksContext();
        }

        private void initLanguageTable() {
            using (var langstream = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("EbookObjects.Misc.langtable.csv"))) {
                var langs = langstream.FromCsv<LangData>().ToArray();
                var existNames = LanguageNames.Select(n => n.Name).ToArray();
                var newNames = new List<Models.LanguageName>(langs.Select(l => l.Name).Distinct().Except(existNames).Select(n => new Models.LanguageName { Name = n }));

                var newCodes = new List<Models.LanguageCode>();
                foreach (var lang in langs) {
                    var langcode = newCodes.SingleOrDefault(c => c.Code == lang.Code);
                    if (langcode == null) langcode = LanguageCodes.SingleOrDefault(c => c.Code == lang.Code);
                    if (langcode == null) {
                        var langName = newNames.SingleOrDefault(n => n.Name == lang.Name);
                        if (langName == null) langName = LanguageNames.SingleOrDefault(n => n.Name == lang.Name);
                        var newCode = new Models.LanguageCode { Code = lang.Code };
                        if (newCode.LanguageNames == null) newCode.LanguageNames = new List<Models.LanguageName>();
                        newCode.LanguageNames.Add(langName);
                        newCodes.Add(newCode);
                    } else if (!langcode.LanguageNames.Any(n => n.Name == lang.Name)) {
                        var langName = newNames.SingleOrDefault(n => n.Name == lang.Name);
                        if (langName == null) langName = LanguageNames.SingleOrDefault(n => n.Name == lang.Name);
                        langcode.LanguageNames.Add(langName);
                    }
                }
                LanguageNames.AddRange(newNames);
                LanguageCodes.AddRange(newCodes);
            }
        }

        private class LangData {
            public string Code { get; set; }
            public string Name { get; set; }
        }

    }

    public class ApplicationUser : IdentityUser {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

}
