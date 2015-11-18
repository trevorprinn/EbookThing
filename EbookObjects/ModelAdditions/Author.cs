using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    partial class Author {

        public static Author Get(EbooksContext db, string name) {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var author = db.Authors.SingleOrDefault(a => a.Name == name);
            if (author == null) {
                db.Authors.Add(author = new Author { Name = name });
            }
            return author;
        }

        public static Author Get(EbooksContext db, Epub ep) {
            return Get(db, ep.Author);
        }

    }
}
