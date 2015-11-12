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

        // These properties returning Bitmaps are mainly for debugging, so the covers can be displayed in LinqPad.

        public Bitmap CoverPic => getBitmap(Picture);

        public Bitmap ThumbnailPic => getBitmap(Thumbnail);

        private Bitmap getBitmap(byte[] data) {
            // Can't dispose of the stream as it has to be kept open to access the bitmap.
            var ms = new MemoryStream(data);
            return new Bitmap(ms);
        }

        public static Cover Get(EbooksContext context, Epub ep) {
            var bcover = ep.GetCoverData();
            if (bcover == null) return null;
            string checksum = bcover.GetChecksum();
            var cover = context.Covers.SingleOrDefault(c => c.Checksum == checksum);
            if (cover == null) {
                using (var ts = new MemoryStream()) {
                    ImageCodecInfo encoder = ImageCodecInfo.GetImageEncoders().Single(e => e.FormatID == ImageFormat.Jpeg.Guid);
                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L);

                    ep.CoverThumbnail.Save(ts, encoder, encoderParams);
                    context.Covers.Add(cover = new Cover {
                        Picture = bcover,
                        Thumbnail = ts.ToArray(),
                        Checksum = checksum
                    });
                }
            }
            return cover;
        }

    }
}
