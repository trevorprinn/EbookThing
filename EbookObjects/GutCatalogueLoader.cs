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
            int count = 0;
            foreach (var book in booksdoc.Root.Elements("Book")) {
                var gutBookId = (int)book.Attribute("Id");
                var gutBook = GutBook.Get(_db, gutBookId);
                gutBook.Title = book.Attribute("Title").Value;
                gutBook.GutAuthor = GutAuthor.Get(_db, book.Attribute("Author")?.Value);
                gutBook.Language = book.Attribute("Language").Value;
                gutBook.EpubUrlNoImages = book.Attribute("EpubUrlNoImages")?.Value;
                gutBook.EpubUrlImages = book.Attribute("EpubUrlImages")?.Value;
                gutBook.ThumbnailUrl = book.Attribute("ThumbnailUrl")?.Value;
                gutBook.CoverUrl = book.Attribute("CoverUrl")?.Value;

                if (++count >= 100) {
                    _db.SaveChanges();
                    count = 0;
                }
            }
            _db.SaveChanges();
        }
    }
}
