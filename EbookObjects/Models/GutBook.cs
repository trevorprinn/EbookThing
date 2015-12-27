using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Net;
using System.IO;
using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;

namespace EbookObjects.Models {
    [Table("GutBook")]
    public class GutBook {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int GutBookId { get; set; }

        public int? GutAuthorId { get; set; }

        public virtual GutAuthor GutAuthor { get; set; }

        public string Title { get; set; }

        [ForeignKey("LanguageCode")]
        [MaxLength(5)]
        public string Language { get; set; }

        public bool StandardEpubUrlNoImages { get; set; }

        public bool StandardEpubUrlImages { get; set; }

        public string EpubUrlNoImages { get; set; }

        public string EpubUrlImages { get; set; }

        public bool StandardThumbnailUrl { get; set; }

        public bool StandardCoverUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public byte[] ThumbnailData { get; set; }

        public string CoverUrl { get; set; }



        public virtual ICollection<EpubFile> EpubFiles { get; set; }

        public virtual LanguageCode LanguageCode { get; set; }


        // Retrieves a file, image or book data, from the url
        private byte[] getFile(string url) {
            var req = (HttpWebRequest)WebRequest.Create(url);
            using (var resp = req.GetResponse())
            using (var m = new MemoryStream()) {
                resp.GetResponseStream().CopyTo(m);
                m.Seek(0, SeekOrigin.Begin);
                return m.ToArray();
            }
        }

        public byte[] GetEpubNoImages() {
            if (!StandardEpubUrlNoImages && string.IsNullOrWhiteSpace(EpubUrlNoImages)) return null;
            string url = StandardEpubUrlNoImages ? $"http://www.gutenberg.org/ebooks/{GutBookId}.epub.noimages" : EpubUrlNoImages;
            return getFile(url);
        }
        public byte[] GetEpubImages() {
            if (!StandardEpubUrlImages && string.IsNullOrWhiteSpace(EpubUrlImages)) return null;
            string url = StandardEpubUrlImages ? $"http://www.gutenberg.org/ebooks/{GutBookId}.epub.images" : EpubUrlImages;
            return getFile(url);
        }

        public byte[] GetThumbnailData() {
            if (ThumbnailData != null) return ThumbnailData;
            if (!StandardThumbnailUrl && string.IsNullOrWhiteSpace(ThumbnailUrl)) return null;
            string url = StandardThumbnailUrl ? $"http://www.gutenberg.org/cache/epub/{GutBookId}/pg{GutBookId}.cover.small.jpg" : ThumbnailUrl;
            ThumbnailData = getFile(url);
            return ThumbnailData;
        }

        public Bitmap GetThumbnail() {
            var data = GetThumbnailData();
            if (data == null) return null;
            return new Bitmap(new MemoryStream(data));
        }

        public byte[] GetCoverData() {
            if (!StandardCoverUrl && string.IsNullOrWhiteSpace(CoverUrl)) return null;
            string url = StandardCoverUrl ? $"http://www.gutenberg.org/cache/epub/{GutBookId}/pg{GutBookId}.cover.medium.jpg" : CoverUrl;
            return getFile(url);
        }

        /// <summary>
        /// Gets the book's language names
        /// </summary>
        [NotMapped]
        public string[] Languages {
            get { return LanguageCode.LanguageNames.Select(ln => ln.Name).OrderBy(ln => ln).ToArray(); }
        }

        public static IQueryable<GutBook> ByLanguage(EbooksContext db, string languageName) {
            if (string.IsNullOrWhiteSpace(languageName)) return db.GutBooks;
            return db.LanguageNames.Where(ln => ln.Name == languageName).SelectMany(ln => ln.LanguageCodes).SelectMany(lc => lc.GutBooks);
        }
    }
}
