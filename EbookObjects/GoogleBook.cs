using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects {
    public class GoogleBook : IDisposable {
        private JToken _data;
        private byte[] _thumbnailData;
        private Bitmap _thumbnail;
        private bool _thumbLoaded;
        private byte[] _coverData;
        private Bitmap _cover;
        private bool _coverLoaded;

        public GoogleBook(JToken data) {
            _data = data;
        }

        protected virtual void Dispose(bool disposing) {
            if (_cover != null) _cover.Dispose();
            if (_thumbnail != null) _thumbnail.Dispose();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static IEnumerable<GoogleBook> GetBooks(
            string title = null,
            string author = null,
            string publisher = null,
            string subject = null,
            string isbn = null,
            string language = null) {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(title)) parts.Add($"intitle:{title}");
            if (!string.IsNullOrWhiteSpace(author)) parts.Add($"inauthor:{author}");
            if (!string.IsNullOrWhiteSpace(publisher)) parts.Add($"inpublisher:{publisher}");
            if (!string.IsNullOrWhiteSpace(subject)) parts.Add($"subject:{subject}");
            if (!string.IsNullOrWhiteSpace(isbn)) parts.Add($"isbn:{isbn}");
            if (!parts.Any()) return new GoogleBook[0];
            string url = $"https://www.googleapis.com/books/v1/volumes?q={string.Join("+", parts)}&key={apiKey}&maxResults=40&projection=full";
            if (!string.IsNullOrWhiteSpace(language)) url += $"&langRestrict={language}";
            var data = retrieve(url);
            int bookCount = Convert.ToInt32(data["totalItems"]);
            if (bookCount == 0) return new GoogleBook[0];
            var books = new List<GoogleBook>(Load(data));
            while (books.Count < bookCount) {
                string contUrl = url + $"&startIndex={books.Count}";
                data = retrieve(contUrl);
                if (data["items"] == null) break;
                books.AddRange(Load(data));
                if (books.Count >= 100) break;
            }
            return books;
        }

        private static JObject retrieve(string url) {
            var req = (HttpWebRequest)WebRequest.Create(url);
            using (var resp = req.GetResponse())
            using (var m = new MemoryStream()) {
                resp.GetResponseStream().CopyTo(m);
                m.Seek(0, SeekOrigin.Begin);
                var data = JObject.Parse(Encoding.UTF8.GetString(m.ToArray()));
                return data;
            }
        }

        private static string apiKey => SecretConfig.GoogleBooksApiKey;

        private JToken info => _data["volumeInfo"];

        private JToken accessInfo => _data["accessInfo"];

        public string Title => info["title"].ToString();

        public IEnumerable<string> Authors => info["authors"]?.Select(a => a.ToString());

        public string Description => info["description"]?.ToString();

        public string VolumeId => _data["id"]?.ToString();

        public string Language => info["language"]?.ToString();

        public string InfoLink => info["infoLink"]?.ToString();

        public IEnumerable<Tuple<string, string>> Identifiers {
            get {
                var ids = new List<Tuple<string, string>>();
                var indIds = info["industryIdentifiers"];
                if (indIds != null) {
                    foreach (var id in info["industryIdentifiers"]) {
                        ids.Add(new Tuple<string, string>(id["type"].ToString(), id["identifier"].ToString()));
                    }
                }
                return ids;
            }
        }

        public string Isbn13 => Identifiers.SingleOrDefault(i => i.Item1 == "ISBN_13")?.Item2;
        public string Isbn10 => Identifiers.SingleOrDefault(i => i.Item1 == "ISBN_10")?.Item2;

        public string Isbn => Isbn13 ?? Isbn10;

        public byte[] ThumbnailData {
            get {
                if (!_thumbLoaded) {
                    _thumbLoaded = true;
                    string url = ThumbnailUrl;
                    if (url != null) _thumbnailData = getCoverData(url);
                }
                return _thumbnailData;
            }
        }

        public string ThumbnailUrl =>
            getCoverLink(new CoverSizes[] { CoverSizes.thumbnail, CoverSizes.smallThumbnail });

        public byte[] CoverData {
            get {
                if (!_coverLoaded) {
                    _coverLoaded = true;
                    CoverSizes[] sizes = { CoverSizes.large, CoverSizes.medium, CoverSizes.extraLarge, CoverSizes.small };
                    string url = getCoverLink(sizes);
                    if (url != null) _coverData = getCoverData(url);
                }
                return _coverData;
            }
        }

        private byte[] getCoverData(string url) {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            using (var resp = req.GetResponse())
            using (var m = new MemoryStream()) {
                resp.GetResponseStream().CopyTo(m);
                m.Seek(0, SeekOrigin.Begin);
                return m.ToArray();
            }
        }

        public static IEnumerable<GoogleBook> Load(JObject data) {
            return data["items"].Select(i => new GoogleBook(i));
        }

        private enum CoverSizes {
            smallThumbnail, thumbnail, small, medium, large, extraLarge
        }

        private string getCoverLink(CoverSizes size) {
            if (info["imageLinks"] == null) return null;
            return info["imageLinks"][size.ToString()]?.ToString();
        }

        private string getCoverLink(params CoverSizes[] sizes) {
            foreach (var size in sizes) {
                string url = getCoverLink(size);
                if (url != null) return url;
            }
            return null;
        }

        public Bitmap Cover {
            get {
                if (CoverData == null) return null;
                if (_cover != null) return _cover;
                var m = new MemoryStream(_coverData);
                var b = new Bitmap(m);
                return (_cover = new Bitmap(m));
            }
        }

        public Bitmap Thumbnail {
            get {
                if (ThumbnailData == null) return null;
                if (_thumbnail != null) return _cover;
                var m = new MemoryStream(_thumbnailData);
                var b = new Bitmap(m);
                return (_thumbnail = new Bitmap(m));
            }
        }
    }
}
