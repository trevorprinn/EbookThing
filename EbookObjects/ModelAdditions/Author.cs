using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    partial class Author {

        public static Author Get(EbooksContext context, Epub ep) {
            if (string.IsNullOrWhiteSpace(ep.Author)) return null;
            var author = context.Authors.SingleOrDefault(a => a.Name == ep.Author);
            if (author == null) {
                context.Authors.Add(author = new Author { Name = ep.Author });
            }
            return author;
        }

    }
}
