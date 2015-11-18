using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    partial class Publisher {

        public static Publisher Get(EbooksContext db, Epub ep) {
            if (string.IsNullOrWhiteSpace(ep.Publisher)) return null;
            var pub = db.Publishers.SingleOrDefault(p => p.Name == ep.Publisher);
            if (pub == null) {
                db.Publishers.Add(pub = new Publisher { Name = ep.Publisher });
            }
            return pub;
        }
    }
}
