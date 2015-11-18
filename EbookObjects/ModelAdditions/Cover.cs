using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    partial class Cover {

        private static byte[] _emptyThumbnail;

        // These properties returning Bitmaps are mainly for debugging, so the covers can be displayed in LinqPad.

        public Bitmap CoverPic => getBitmap(Picture);

        public Bitmap ThumbnailPic => getBitmap(Thumbnail);

        private Bitmap getBitmap(byte[] data) {
            // Can't dispose of the stream as it has to be kept open to access the bitmap.
            var ms = new MemoryStream(data);
            return new Bitmap(ms);
        }

        public static Cover Get(EbooksContext db, Epub ep) {
            var bcover = ep.GetCoverData();
            if (bcover == null) return null;
            string checksum = bcover.GetChecksum();
            var cover = db.Covers.SingleOrDefault(c => c.Checksum == checksum);
            if (cover == null) {
                using (var ts = new MemoryStream()) {
                    ImageCodecInfo encoder = ImageCodecInfo.GetImageEncoders().Single(e => e.FormatID == ImageFormat.Jpeg.Guid);
                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L);

                    ep.CoverThumbnail.Save(ts, encoder, encoderParams);
                    db.Covers.Add(cover = new Cover {
                        Picture = bcover,
                        Thumbnail = ts.ToArray(),
                        Checksum = checksum
                    });
                }
            }
            return cover;
        }

        public static byte[] EmptyThumbnail {
            get {
                const int h = 100;
                const int w = 70;
                if (_emptyThumbnail == null) {
                    using (var b = new Bitmap(w, h))
                    using (var g = Graphics.FromImage(b)) {
                        g.FillRectangle(Brushes.LightGray, 0, 0, w, h);
                        g.DrawLine(Pens.Red, 0, 0, w, h);
                        g.DrawLine(Pens.Red, 0, h, w, 0);
                        ImageCodecInfo encoder = ImageCodecInfo.GetImageEncoders().Single(e => e.FormatID == ImageFormat.Jpeg.Guid);
                        var encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L);
                        using (var m = new MemoryStream()) {
                            b.Save(m, encoder, encoderParams);
                            _emptyThumbnail = m.ToArray();
                        }
                    }
                }
                return _emptyThumbnail;
            }
        }

        public static byte[] EmptyCover => EmptyThumbnail;

        public string ImageContentType => "image/jpeg";
    }
}
