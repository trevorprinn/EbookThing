using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects {
    /// <summary>
    /// Given an ISBN (10 or 13) this will search for covers on openlibrary.org
    /// </summary>
    public class OpenCover : IDisposable {
        public string Isbn { get; }
        private byte[] _thumbnailData;
        private byte[] _coverData;
        private Bitmap _thumbnail;
        private Bitmap _cover;
        private bool _thumbnailLoaded;
        private bool _coverLoaded;

        public OpenCover(string isbn) {
            Isbn = isbn;
        }

        protected virtual void Dispose(bool disposing) {
            if (_thumbnail != null) _thumbnail.Dispose();
            if (_cover != null) _cover.Dispose();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public byte[] ThumbnailData {
            get {
                if (!_thumbnailLoaded) {
                    _thumbnailLoaded = true;
                    getCover('M', out _thumbnailData, out _thumbnail);
                }
                return _thumbnailData;
            }
        }

        public byte[] CoverData {
            get {
                if (!_coverLoaded) {
                    _coverLoaded = true;
                    getCover('L', out _coverData, out _cover);
                }
                return _coverData;
            }
        }

        public Bitmap Thumbnail {
            get {
                if (!_thumbnailLoaded) {
                    var x = ThumbnailData;
                }
                return _thumbnail;
            }
        }

        public Bitmap Cover {
            get {
                if (!_coverLoaded) {
                    var x = CoverData;
                }
                return _cover;
            }
        }

        private void getCover(char size, out byte[] data, out Bitmap pic) {
            string url = $"http://covers.openlibrary.org/b/isbn/{Isbn}-{size}.jpg";
            var req = (HttpWebRequest)WebRequest.Create(url);
            using (var resp = req.GetResponse()) {
                var m = new MemoryStream();
                resp.GetResponseStream().CopyTo(m);
                m.Seek(0, SeekOrigin.Begin);
                data = m.ToArray();
                pic = new Bitmap(m);
                if (pic.Height == 1) {
                    pic.Dispose();
                    pic = null;
                    data = null;
                }
            }
        }
    }
}
