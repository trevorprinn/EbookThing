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
    public class GutBook {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int GutBookId { get; set; }

        public int? GutAuthorId { get; set; }

        public virtual GutAuthor GutAuthor { get; set; }

        public string Title { get; set; }

        [MaxLength(5)]
        public string Language { get; set; }

        public string EpubUrlNoImages { get; set; }

        public string EpubUrlImages { get; set; }

        public string ThumbnailUrl { get; set; }

        public byte[] ThumbnailData { get; set; }

        public string CoverUrl { get; set; }

        public virtual ICollection<EpubFile> EpubFiles { get; set; }

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
            if (string.IsNullOrWhiteSpace(EpubUrlNoImages)) return null;
            return getFile(EpubUrlNoImages);
        }
        public byte[] GetEpubImages() {
            if (string.IsNullOrWhiteSpace(EpubUrlImages)) return null;
            return getFile(EpubUrlImages);
        }

        public byte[] GetThumbnailData() {
            if (ThumbnailData != null) return ThumbnailData;
            if (string.IsNullOrWhiteSpace(ThumbnailUrl)) return null;
            ThumbnailData = getFile(ThumbnailUrl);
            return ThumbnailData;
        }

        public Bitmap GetThumbnail() {
            var data = GetThumbnailData();
            if (data == null) return null;
            return new Bitmap(new MemoryStream(data));
        }

        public byte[] GetCoverData() {
            if (string.IsNullOrWhiteSpace(CoverUrl)) return null;
            return getFile(CoverUrl);
        }

        public static GutBook Get(EbooksContext db, int gutBookId) {
            var book = db.GutBooks.SingleOrDefault(b => b.GutBookId == gutBookId);
            if (book == null) db.GutBooks.Add(new GutBook { GutBookId = gutBookId });
            return book;
        }
    }
}
