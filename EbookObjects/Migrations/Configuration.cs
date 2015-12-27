namespace EbookObjects.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
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

            // Make sure cover contents types have been set
            foreach (var cover in context.Covers.Where(c => c.ContentType == null)) {
                using (var ep = new Epub(cover.Books.FirstOrDefault())) {
                    if (ep != null) {
                        cover.ContentType = ep.CoverContentType;
                    }
                }
            }

            // Load the data in langtable.csv
            using (var langstream = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("EbookObjects.Misc.langtable.csv"))) {
                var langs = langstream.FromCsv<LangData>().ToArray();
                var existNames = context.LanguageNames.Select(n => n.Name).ToArray();
                var newNames = new List<Models.LanguageName>(langs.Select(l => l.Name).Distinct().Except(existNames).Select(n => new Models.LanguageName { Name = n }));

                var newCodes = new List<Models.LanguageCode>();
                foreach (var lang in langs) {
                    var langcode = newCodes.SingleOrDefault(c => c.Code == lang.Code);
                    if (langcode == null) langcode = context.LanguageCodes.SingleOrDefault(c => c.Code == lang.Code);
                    if (langcode == null) {
                        var langName = newNames.SingleOrDefault(n => n.Name == lang.Name);
                        if (langName == null) langName = context.LanguageNames.SingleOrDefault(n => n.Name == lang.Name);
                        var newCode = new Models.LanguageCode { Code = lang.Code };
                        if (newCode.LanguageNames == null) newCode.LanguageNames = new Collection<Models.LanguageName>();
                        newCode.LanguageNames.Add(langName);
                        newCodes.Add(newCode);
                    } else if (!langcode.LanguageNames.Any(n => n.Name == lang.Name)) {
                        var langName = newNames.SingleOrDefault(n => n.Name == lang.Name);
                        if (langName == null) langName = context.LanguageNames.SingleOrDefault(n => n.Name == lang.Name);
                        langcode.LanguageNames.Add(langName);
                    }
                }
                context.LanguageNames.AddRange(newNames);
                context.LanguageCodes.AddRange(newCodes);
            }
        }

        private class LangData {
            public string Code { get; set; }
            public string Name { get; set; }
        }
    }
}
