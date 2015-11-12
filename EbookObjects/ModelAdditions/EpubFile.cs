using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    partial class EpubFile {

        public static EpubFile Get(EbooksContext context, Epub ep) {
            using (var s = ep.GetDataStream()) {
                string checksum = s.GetChecksum();
                var epub = context.EpubFiles.SingleOrDefault(ef => ef.Checksum == checksum);
                if (epub == null) {
                    using (var m = new MemoryStream()) {
                        s.Seek(0, SeekOrigin.Begin);
                        s.CopyTo(m);

                        context.EpubFiles.Add(epub = new EpubFile {
                            Contents = m.ToArray(),
                            Checksum = checksum
                        });
                    }
                }
                return epub;
            }
        }

    }
}
