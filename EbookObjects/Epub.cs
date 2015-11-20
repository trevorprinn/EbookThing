using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EbookObjects {

    /// <summary>
    /// Extracts data from an epub file.
    /// </summary>
    /// <remarks>
    /// An epub file is a zip.
    /// </remarks>
    public class Epub : IDisposable {
        // The opf, essentially the index containing the metadata of the epb file.
        private XDocument _opf;

        // The general metadata within the opf
        private XElement _metadata;

        // Various namespaces used within the opf
        private static XNamespace _ns = @"http://www.idpf.org/2007/opf";
        private static XNamespace _dc = @"http://purl.org/dc/elements/1.1/";
        private static XNamespace _nsopf = @"http://www.idpf.org/2007/opf";
        private static XNamespace _calibre = @"http://calibre.kovidgoyal.net/2009/metadata";

        // The root folder, defined in the container xml (which should always be in META-INF/container.xml)
        // This is the folder where the opf file is stored, and is used as the root for internal hrefs.
        private string _rootFolder;

        // Flags that the cover has been loaded and doesn't need to be reloaded if accessed again
        private bool _coverLoaded;

        // Caches the book cover, if there is one.
        private Bitmap _cover;

        // The epub file data as a zip archive
        private ZipArchive _zip;

        // The epub file data as a stream (this stream must support Seeking).
        private Stream _epubContents;

        // The cover data
        private MemoryStream _coverStream;

        /// <summary>
        /// Opens an epub file.
        /// </summary>
        /// <param name="pathname"></param>
        public Epub(string pathname) : this(new FileStream(pathname, FileMode.Open, FileAccess.Read)) { }

        /// <summary>
        /// Opens the epub file contained in the stream
        /// </summary>
        /// <param name="epubContents">This stream must be seekable</param>
        public Epub(Stream epubContents) {
            _zip = new ZipArchive(_epubContents = epubContents);
            var containerEntry = _zip.Entries.SingleOrDefault(e => e.FullName == @"META-INF/container.xml");
            if (containerEntry == null) throw new EpubException("Epub has no container");
            var container = XDocument.Load(containerEntry.Open());
            var ns = container.Root.GetDefaultNamespace();
            var opfName = (string)container.Root.Element(ns + "rootfiles").Element(ns + "rootfile").Attribute("full-path");

            var opfEntry = _zip.Entries.SingleOrDefault(e => e.FullName == opfName);
            if (opfEntry == null) throw new EpubException($"Epub has no opf ({opfName})");
            try {
                _opf = XDocument11.Load(opfEntry.Open());
                _metadata = _opf.Root.Element(_ns + "metadata");
            } catch (Exception ex) {
                throw new EpubException("Error loading epub metadata", ex);
            }
            _rootFolder = Path.GetDirectoryName(opfName);
        }

        /// <summary>
        /// Opens an epub file from a database Book entity.
        /// </summary>
        /// <param name="book"></param>
        public Epub(Models.Book book) : this(new MemoryStream(book.EpubFile.Contents)) { }

        protected virtual void Dispose(bool disposing) {
            if (_coverStream != null) _coverStream.Dispose();
            if (_zip != null) _zip.Dispose();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the title of the book.
        /// </summary>
        public string Title => _metadata?.Element(_dc + "title")?.Value;

        /// <summary>
        /// Gets the author of the book.
        /// </summary>
        public string Author => _metadata?.Element(_dc + "creator")?.Value;

        /// <summary>
        /// Gets the publisher of the book.
        /// </summary>
        public string Publisher => _metadata?.Element(_dc + "publisher")?.Value;

        /// <summary>
        /// Gets the ids of the book (isbn, Amazon id etc). Item 1 is the id name (which may be empty), Item 2 is the id value.
        /// </summary>
        public IEnumerable<Tuple<string, string>> Identifiers => _metadata?.Elements(_dc + "identifier").Select(e => new Tuple<string, string>(e.Attribute(_nsopf + "scheme")?.Value ?? "", e.Value));

        /// <summary>
        /// Gets the tags (subjects) of the book.
        /// </summary>
        public IEnumerable<string> Tags => _metadata?.Elements(_dc + "subject").Select(e => e.Value).Distinct();

        /// <summary>
        /// Gets the manifest record from the opf file.
        /// </summary>
        private XElement manifest => _opf.Root.Element(_ns + "manifest");

        /// <summary>
        /// Gets the series the book belongs to.
        /// </summary>
        public string Series => _metadata?.Elements(_ns + "meta")?.FirstOrDefault(e => e.Attribute("name")?.Value == "calibre:series")?.Attribute("content")?.Value;

        /// <summary>
        /// If the book is part of a series, gets the number within the series.
        /// </summary>
        public decimal? SeriesNbr {
            get {
                if (Series == null) return null;
                string nbr = _metadata?.Elements(_ns + "meta")?.FirstOrDefault(e => e.Attribute("name")?.Value == "calibre:series_index")?.Attribute("content")?.Value;
                if (nbr == null) return null;
                return Convert.ToDecimal(nbr);
            }
        }

        /// <summary>
        /// Gets the description of the book. This may be text or an html fragment.
        /// </summary>
        public string Description => _metadata?.Element(_dc + "description")?.Value;

        private void getCoverInfo(out string file, out string contentType) {
            file = null;
            // Check in the metadata
            string manEntry = (string)_metadata?.Elements(_ns + "meta")?.FirstOrDefault(e => (string)e.Attribute("name") == "cover")?.Attribute("content");
            if (manEntry != null) file = manifest?.Elements(_ns + "item").SingleOrDefault(e => (string)e.Attribute("id") == manEntry)?.Attribute("href")?.Value;
            if (file != null && getMediaType(file) != "image") file = findCoverInHtml(file);
            if (file == null) {
                // Check in the guide
                file = _opf.Root.Element(_ns + "guide")?.Elements(_ns + "reference").SingleOrDefault(e => (string)e.Attribute("type") == "cover")?.Attribute("href")?.Value;
                if (file != null && getMediaType(file) != "image") file = findCoverInHtml(file);
            }
            if (file == null) {
                // Check in the manifest
                file = manifest?.Elements(_ns + "item")?.FirstOrDefault(e => (string)e.Attribute("id") == "cover")?.Attribute("href")?.Value;
                if (file != null && getMediaType(file) != "image") file = findCoverInHtml(file);
            }
            contentType = getFullMediaType(file);
        }

        /// <summary>
        /// In some books the cover is an html file. This routine checks if it is an html file and tries
        /// to look inside it to find the cover graphic file.
        /// </summary>
        /// <param name="file">The file defined as the cover.</param>
        /// <returns>The file containing the graphic, if one is found.</returns>
        private string findCoverInHtml(string file) {
            // Covers are sometimes in an html wrapper.
            XDocument doc = openDoc(getFullPath(file));
            if (doc == null) return null;
            var img = doc.Root.Descendants(doc.Root.GetDefaultNamespace() + "img").FirstOrDefault(i => (string)i.Attribute("alt") == "cover")?.Attribute("src")?.Value;
            if (img == null) {
                // Used in some books
                XNamespace xlink = "http://www.w3.org/1999/xlink";
                XNamespace svg = "http://www.w3.org/2000/svg";
                img = doc.Root.Descendants(svg + "image")?.FirstOrDefault()?.Attribute(xlink + "href")?.Value;
            }
            if (img == null) return null;
            return Path.Combine(Path.GetDirectoryName(file), img).Replace('\\', '/');
        }

        /// <summary>
        /// Tries to open a file within the zip as an XML document.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Null if the file is not found or is not an XML document.</returns>
        private XDocument openDoc(string path) {
            var entry = _zip.Entries.SingleOrDefault(e => e.FullName == path);
            if (entry == null) return null;
            using (var s = entry.Open()) {
                try {
                    return XDocument11.Load(s);
                } catch { return null; }
            }
        }

        /// <summary>
        /// Gets the first part of the media type declaration for an item from the manifest
        /// given the href of the item.
        /// </summary>
        /// <param name="href"></param>
        /// <returns></returns>
        private string getMediaType(string href) {
            string mtype = getFullMediaType(href);
            if (mtype == null) return null;
            return mtype.Split('/')[0];
        }

        private string getFullMediaType(string href) {
            return href == null ? null : manifest?.Elements(_ns + "item").SingleOrDefault(m => (string)m.Attribute("href") == href)?.Attribute("media-type")?.Value;
        }

        /// <summary>
        /// Converts a relative href to a full path within the zip (ie relative to the opf file location).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string getFullPath(string path) =>
            string.IsNullOrEmpty(_rootFolder) ? path : _rootFolder + '/' + path;

        /// <summary>
        /// Gets the cover of the book.
        /// </summary>
        public Bitmap Cover {
            get {
                if (_coverLoaded) return _cover;
                _coverLoaded = true;
                var coverData = GetCoverData();
                if (coverData == null) return null;
                // Can't dispose the stream here because it has to be kept open as long as the bitmap is in use
                _coverStream = new MemoryStream(coverData, false);
                try {
                    return (_cover = new Bitmap(_coverStream));
                } catch { return null; }
            }
        }

        /// <summary>
        /// Gets the cover of the book as a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] GetCoverData() {
            string coverFile, contentType;
            getCoverInfo(out coverFile, out contentType);
            if (coverFile == null) return null;
            var entry = _zip.Entries.SingleOrDefault(e => e.FullName == coverFile);
            if (entry == null) return null;
            using (var s = entry.Open())
            using (var m = new MemoryStream()) {
                s.CopyTo(m);
                m.Seek(0, SeekOrigin.Begin);
                return m.ToArray();
            }
        }

        public string CoverContentType {
            get {
                string f, t;
                getCoverInfo(out f, out t);
                return t;
            }
        }

        /// <summary>
        /// Creates a thumbnail of the cover.
        /// </summary>
        /// <param name="size">The size to use for the longest side (defaults to 100 pixels)</param>
        /// <returns></returns>
        public Bitmap GetCoverThumbnail(int size = 100) {
            if (Cover == null) return null;
            // Set the size of the larger edge
            double factor = (double)size / Math.Max(Cover.Width, Cover.Height);
            return (Bitmap)Cover.GetThumbnailImage((int)(Cover.Width * factor), (int)(Cover.Height * factor), null, IntPtr.Zero);
        }

        /// <summary>
        /// Gets a thumbnail with the largest side being 100 pixels.
        /// </summary>
        public Bitmap CoverThumbnail => GetCoverThumbnail();

        /// <summary>
        /// Gets a stream containing the entire epub file.
        /// </summary>
        /// <returns></returns>
        public Stream GetDataStream() => _epubContents;

        /// <summary>
        /// Gets the opf (primarily for debugging in Linqpad).
        /// </summary>
        /// <returns></returns>
        public XDocument GetOpf() => new XDocument(_opf);

        /// <summary>
        /// Outputs all of the files in the manifest to the given location.
        /// </summary>
        /// <param name="foldername"></param>
        /// <remarks>
        /// The output folder is created if it does not exist.
        /// </remarks>
        public void ExportManifest(string foldername) {
            if (!Directory.Exists(foldername)) Directory.CreateDirectory(foldername);
            foreach (var item in manifest.Elements(_ns + "item")) {
                var arc = _zip.Entries.SingleOrDefault(a => a.FullName == getFullPath(item.Attribute("href").Value));
                if (arc != null) {
                    string outname = Path.Combine(foldername, item.Attribute("href").Value);
                    string folder = Path.GetDirectoryName(outname);
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    using (var s = new FileStream(outname, FileMode.Create, FileAccess.Write))
                    using (var sa = arc.Open()) {
                        sa.CopyTo(s);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the contents of one of the files from the zip.
        /// </summary>
        /// <param name="path">Relative to the root folder</param>
        /// <returns></returns>
        public Stream GetContentFile(string path) {
            var arc = _zip.Entries.SingleOrDefault(a => a.FullName == getFullPath(path));
            if (arc == null) return new MemoryStream(0);
            var m = new MemoryStream();
            arc.Open().CopyTo(m);
            m.Seek(0, SeekOrigin.Begin);
            return m;
        }

        /// <summary>
        /// Gets a list of the files that make up the contents of the book. This can be used with ExportManifest to
        /// display the book.
        /// </summary>
        public IEnumerable<string> SpineRefs => _opf.Root.Element(_ns + "spine")?.Elements(_ns + "itemref")
            .Select(r => manifest.Elements(_ns + "item").Single(i => i.Attribute("id").Value == r.Attribute("idref").Value).Attribute("href").Value);

        /// <summary>
        /// Gets the table of contents.
        /// </summary>
        public Toc Toc {
            get {
                string tocid = _opf.Root.Element(_ns + "spine")?.Attribute("toc")?.Value ?? "ncx";
                string tocref = manifest?.Elements(_ns + "item")?.FirstOrDefault(i => i.Attribute("id").Value == tocid)?.Attribute("href").Value;
                if (tocref == null) return null;
                var arc = _zip.Entries.SingleOrDefault(a => a.FullName == getFullPath(tocref));
                if (arc == null) return null;
                using (var sa = arc.Open()) return new Toc(XDocument11.Load(sa));
            }
        }
    }

    /// <summary>
    /// A book's Table of Contents.
    /// </summary>
    public class Toc {
        private XElement _toc;
        private static XNamespace _ns = "http://www.daisy.org/z3986/2005/ncx/";

        internal Toc(XDocument toc) {
            _toc = toc.Root;
        }

        /// <summary>
        /// Gets the title of the Table of Contents.
        /// </summary>
        public string Title => _toc.Element(_ns + "docTitle")?.Element(_ns + "text")?.Value;

        /// <summary>
        /// One navigation point within the Table of Contents.
        /// </summary>
        public class NavPoint {
            private XElement _navPoint;
            internal NavPoint(XElement navPoint) {
                _navPoint = navPoint;
            }

            /// <summary>
            /// Gets the text to display for the navigation point (eg Chapter 2).
            /// </summary>
            public string Text => _navPoint.Element(Toc._ns + "navLabel")?.Element(Toc._ns + "text")?.Value;

            /// <summary>
            /// Gets the href (relative to the opf) of the document pointed to by the navigation point.
            /// </summary>
            public string Ref => _navPoint.Element(Toc._ns + "content")?.Attribute("src")?.Value;
        }

        /// <summary>
        /// Gets the navigation points of the Table of Contents.
        /// </summary>
        public IEnumerable<NavPoint> NavPoints => _toc.Element(_ns + "navMap").Elements(_ns + "navPoint").OrderBy(p => Convert.ToInt32(p.Attribute("playOrder").Value))
            .Select(p => new NavPoint(p));
    }
    

    [Serializable]
    public class EpubException : ApplicationException {
        public EpubException(string message) : base(message) { }
        public EpubException(string message, Exception ex) : base(message, ex) { }
    }
}
