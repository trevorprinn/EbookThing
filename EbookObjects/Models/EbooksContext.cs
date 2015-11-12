using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EbookObjects.Models.Mapping;

namespace EbookObjects.Models
{
    public partial class EbooksContext : DbContext
    {
        static EbooksContext()
        {
            Database.SetInitializer<EbooksContext>(null);
        }

        public EbooksContext()
            : base("Name=EbooksContext")
        {
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
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
        }
    }
}
