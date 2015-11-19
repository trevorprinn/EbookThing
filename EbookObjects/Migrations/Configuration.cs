namespace EbookObjects.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EbookObjects.Models.EbooksContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "EbookObjects.Models.EbooksContext";
        }

        protected override void Seed(EbookObjects.Models.EbooksContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            foreach (var cover in context.Covers.Where(c => c.ContentType == null)) {
                using (var ep = new Epub(cover.Books.FirstOrDefault())) {
                    if (ep != null) {
                        cover.ContentType = ep.CoverContentType;
                    }
                }
            }
        }
    }
}
