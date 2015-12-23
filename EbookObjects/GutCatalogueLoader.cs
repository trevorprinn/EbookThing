using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookObjects.Models;
using System.Xml.Linq;
using System.IO;

namespace EbookObjects {
    /// <summary>
    /// Loads or reloads the Gutenberg catalogue from a CSV file.
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

        public void Load(Stream input) {

            int count = 0;
            var newBooks = new List<GutBook>();
            var newAuthors = new List<GutAuthor>();

            using (var reader = new StreamReader(input)) {
                foreach (var book in reader.FromCsv<BookData>()) {
                    var gutBook = _db.GutBooks.SingleOrDefault(gb => gb.GutBookId == book.Id);
                    if (gutBook == null) {
                        gutBook = new GutBook { GutBookId = book.Id };
                        newBooks.Add(gutBook);
                    }
                    gutBook.Title = book.Title;
                    var gutAuthor = newAuthors.SingleOrDefault(a => a.Name == book.Author);
                    if (gutAuthor == null) gutAuthor = _db.GutAuthors.SingleOrDefault(a => a.Name == book.Author);
                    if (gutAuthor == null && book.Author != null) {
                        gutAuthor = new GutAuthor { Name = book.Author };
                        newAuthors.Add(gutAuthor);
                    }
                    gutBook.GutAuthor = gutAuthor;
                    gutBook.Language = book.Language;
                    gutBook.StandardEpubUrlNoImages = book.StandardEpubUrlNoImages;
                    gutBook.StandardEpubUrlImages = book.StandardEpubUrlImages;
                    gutBook.EpubUrlNoImages = book.EpubUrlNoImages;
                    gutBook.EpubUrlImages = book.EpubUrlImages;
                    gutBook.StandardThumbnailUrl = book.StandardThumbnailUrl;
                    gutBook.StandardCoverUrl = book.StandardCoverUrl;
                    gutBook.ThumbnailUrl = book.ThumbnailUrl;
                    gutBook.CoverUrl = book.CoverUrl;

                    if (++count >= 1000) {
                        _db.GutAuthors.AddRange(newAuthors);
                        newAuthors.Clear();
                        _db.GutBooks.AddRange(newBooks);
                        newBooks.Clear();
                        _db.SaveChanges();
                        count = 0;
                    }
                }
            }

            _db.GutAuthors.AddRange(newAuthors);
            _db.GutBooks.AddRange(newBooks);
            _db.SaveChanges();
        }

        private class BookData {
            public int Id { get; set;}
            public string Title { get; set; }
            public string Author { get; set; }
            public string Language { get; set; }
            public bool StandardEpubUrlNoImages { get; set; }
            public bool StandardEpubUrlImages { get; set; }
            public string EpubUrlNoImages { get; set; }
            public string EpubUrlImages { get; set; }
            public bool StandardThumbnailUrl { get; set; }
            public bool StandardCoverUrl { get; set; }
            public string ThumbnailUrl { get; set; }
            public string CoverUrl { get; set; }
        }
    }
}
