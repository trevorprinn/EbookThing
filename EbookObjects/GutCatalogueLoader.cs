using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookObjects.Models;
using System.Xml.Linq;

namespace EbookObjects {
    /// <summary>
    /// Loads or reloads the Gutenberg catalogue
    /// </summary>
    /// <remarks>
    /// The catalogue is obtained from http://www.gutenberg.org/cache/epub/feeds/rdf-files.tar.zip and processed
    /// using a Linqpad script to reduce the size before uploading to the site.
    /// </remarks>
    public class GutCatalogueLoader : IDisposable {
        private EbooksContext _db;

        public GutCatalogueLoader() {
            _db = new EbooksContext();
        }

        protected virtual void Dispose(bool disposing) {
            _db.Dispose();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Load(XDocument booksdoc) {

            loadAuthors(booksdoc);

            int count = 0;
            var newBooks = new List<GutBook>();
            foreach (var book in booksdoc.Root.Elements("Book")) {
                var gutBookId = (int)book.Attribute("Id");
                var gutBook = _db.GutBooks.SingleOrDefault(gb => gb.GutBookId == gutBookId);
                if (gutBook == null) {
                    gutBook = new GutBook { GutBookId = gutBookId };
                    newBooks.Add(gutBook);
                }
                gutBook.Title = book.Attribute("Title").Value;
                string author = book.Attribute("Author")?.Value;
                gutBook.GutAuthor = author == null ? null : _db.GutAuthors.Single(a => a.Name == author);
                gutBook.Language = book.Attribute("Language").Value;
                gutBook.EpubUrlNoImages = book.Attribute("EpubUrlNoImages")?.Value;
                gutBook.EpubUrlImages = book.Attribute("EpubUrlImages")?.Value;
                gutBook.ThumbnailUrl = book.Attribute("ThumbnailUrl")?.Value;
                gutBook.CoverUrl = book.Attribute("CoverUrl")?.Value;

                if (++count >= 1000) {
                    _db.GutBooks.AddRange(newBooks);
                    newBooks.Clear();
                    _db.SaveChanges();
                    count = 0;
                }
            }
            _db.GutBooks.AddRange(newBooks);
            _db.SaveChanges();
        }

        private void loadAuthors(XDocument booksdoc) {
            var toAdd = booksdoc.Root.Elements("Book").Select(r => r.Attribute("Author")?.Value).Distinct()
                .Where(a => a != null).Except(_db.GutAuthors.Select(ga => ga.Name)).Select(a => new GutAuthor { Name = a });
            _db.GutAuthors.AddRange(toAdd);
            _db.SaveChanges();
        }
    }
}
