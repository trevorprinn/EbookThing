using EbookObjects.Models;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EbookObjects {
    /// <summary>
    /// Downloads a catalogue from Gutenberg and loads it into the database.
    /// </summary>
    public class GutCatalogueDownloader {
        public string DownloadUrl { get; }

        public GutCatalogueDownloader(string downloadUrl = "http://www.gutenberg.org/cache/epub/feeds/rdf-files.tar.zip") {
            DownloadUrl = downloadUrl;
        }

        public void Run() {
            var docs = getRdfFiles()
                .Where(f => f.Id.NotIn(0, 999999) && f.Title != null)
                // Don't include any that have no epubs (eg sound files)
                .Where(f => f.EpubUrlImages != null || f.EpubUrlNoImages != null);
            using (var db = new EbooksContext()) {
                int count = 0;
                var newBooks = new List<GutBook>();
                var newAuthors = new List<GutAuthor>();
                foreach (var doc in docs) {
                    loadBookData(db, doc, newBooks, newAuthors);
                    if (++count >= 1000) {
                        db.GutAuthors.AddRange(newAuthors);
                        newAuthors.Clear();
                        db.GutBooks.AddRange(newBooks);
                        newBooks.Clear();
                        db.SaveChanges();
                        count = 0;
                    }
                }
                db.GutAuthors.AddRange(newAuthors);
                db.GutBooks.AddRange(newBooks);
                db.SaveChanges();
            }
        }

        private void loadBookData(EbooksContext db, GutCatDoc doc, IList<GutBook> newBooks, IList<GutAuthor> newAuthors) {
            var gutBook = db.GutBooks.SingleOrDefault(b => b.GutBookId == doc.Id);
            if (gutBook == null) {
                gutBook = new GutBook { GutBookId = doc.Id };
                newBooks.Add(gutBook);
            }
            gutBook.Title = doc.Title;
            var gutAuthor = newAuthors.SingleOrDefault(a => a.Name == doc.Author);
            if (gutAuthor == null) gutAuthor = db.GutAuthors.SingleOrDefault(a => a.Name == doc.Author);
            if (gutAuthor == null && doc.Author != null) {
                gutAuthor = new GutAuthor { Name = doc.Author };
                newAuthors.Add(gutAuthor);
            }
            gutBook.GutAuthor = gutAuthor;
            gutBook.Language = doc.Language;

            var getUrl = new Func<string, string, string>((url, standard) => url != null && url != standard ? url : null);
            gutBook.EpubUrlImages = getUrl(doc.EpubUrlImages, doc.StandardEpubUrlImages);
            gutBook.StandardEpubUrlImages = doc.EpubUrlImages == doc.StandardEpubUrlImages;
            gutBook.EpubUrlNoImages = getUrl(doc.EpubUrlNoImages, doc.StandardEpubUrlNoImages);
            gutBook.StandardEpubUrlNoImages = doc.EpubUrlNoImages == doc.StandardEpubUrlNoImages;
            gutBook.ThumbnailUrl = getUrl(doc.ThumbnailUrl, doc.StandardThumbnailUrl);
            gutBook.StandardThumbnailUrl = doc.ThumbnailUrl == doc.StandardThumbnailUrl;
            gutBook.CoverUrl = getUrl(doc.CoverUrl, doc.StandardCoverUrl);
            gutBook.StandardCoverUrl = doc.CoverUrl == doc.StandardCoverUrl;
        }

        private IEnumerable<GutCatDoc> getRdfFiles() {
            var req = (HttpWebRequest)WebRequest.Create(DownloadUrl);
            using (var resp = (HttpWebResponse)req.GetResponse()) {
                using (var zip = new ZipInputStream(resp.GetResponseStream())) {
                    zip.GetNextEntry();
                    using (var tar = new TarInputStream(zip)) {
                        TarEntry tarentry;
                        while ((tarentry = tar.GetNextEntry()) != null) {
                            if (tarentry.IsDirectory) continue;
                            yield return new GutCatDoc(tar);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Extracts data from a PG rdf file.
    /// </summary>
    internal class GutCatDoc {
        private XDocument _doc;

        private static XNamespace _pgterms = "http://www.gutenberg.org/2009/pgterms/";
        private static XNamespace _cc = "http://web.resource.org/cc/";
        private static XNamespace _rdfs = "http://www.w3.org/2000/01/rdf-schema#";
        private static XNamespace _dcterms = "http://purl.org/dc/terms/";
        private static XNamespace _rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        private static XNamespace _dcam = "http://purl.org/dc/dcam/";

        public GutCatDoc(string path) {
            _doc = XDocument.Load(path);
        }

        public GutCatDoc(Stream s) {
            _doc = XDocument.Load(s);
        }

        private XElement top => _doc.Root.Element(_pgterms + "ebook");

        public int Id => Convert.ToInt32(top.Attribute(_rdf + "about").Value.Substring(7));

        public string Author => top.Element(_dcterms + "creator")?.Descendants(_pgterms + "name")?.FirstOrDefault()?.Value;

        public string Title => top.Element(_dcterms + "title")?.Value;

        public string Language => top.Element(_dcterms + "language")?.Descendants(_rdf + "value").FirstOrDefault()?.Value;

        private IEnumerable<string> epubUrls => formats("application/epub+zip")
                ?.Select(f => f.Element(_pgterms + "file")?.Attribute(_rdf + "about")?.Value);

        public string StandardEpubUrlNoImages => $"http://www.gutenberg.org/ebooks/{Id}.epub.noimages";

        public string StandardEpubUrlImages => $"http://www.gutenberg.org/ebooks/{Id}.epub.images";

        public string EpubUrlNoImages => epubUrls.SingleOrDefault(u => u.EndsWith("epub.noimages"));

        public string EpubUrlImages => epubUrls.SingleOrDefault(u => u.EndsWith("epub.images"));

        public IEnumerable<XElement> formats(string type) =>
            top.Elements(_dcterms + "hasFormat")?.Where(f => f.Element(_pgterms + "file")?.Element(_dcterms + "format")?.Element(_rdf + "Description")?.Element(_rdf + "value").Value == type);

        private IEnumerable<string> coverUrls => formats("image/jpeg")
            ?.Select(f => f.Element(_pgterms + "file")?.Attribute(_rdf + "about")?.Value).Where(u => u.Contains("cover"));

        public string StandardThumbnailUrl => $"http://www.gutenberg.org/cache/epub/{Id}/pg{Id}.cover.small.jpg";

        public string StandardCoverUrl => $"http://www.gutenberg.org/cache/epub/{Id}/pg{Id}.cover.medium.jpg";

        public string ThumbnailUrl => coverUrls.SingleOrDefault(u => u.EndsWith("small.jpg"));

        public string CoverUrl => coverUrls.SingleOrDefault(u => u.EndsWith("medium.jpg"));
    }
}
