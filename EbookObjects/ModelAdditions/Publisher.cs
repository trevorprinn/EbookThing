using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    partial class Publisher {

        public static Publisher Get(EbooksContext db, string name) {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var pub = db.Publishers.SingleOrDefault(p => p.Name == name);
            if (pub == null) {
                db.Publishers.Add(pub = new Publisher { Name = name });
            }
            return pub;
        }

        public static Publisher Get(EbooksContext db, Epub ep) {
            return Get(db, ep.Publisher);
        }
    }
}
