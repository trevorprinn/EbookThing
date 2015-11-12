using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    partial class Publisher {

        public static Publisher Get(EbooksContext context, Epub ep) {
            if (string.IsNullOrWhiteSpace(ep.Publisher)) return null;
            var pub = context.Publishers.SingleOrDefault(p => p.Name == ep.Publisher);
            if (pub == null) {
                context.Publishers.Add(pub = new Publisher { Name = ep.Publisher });
            }
            return pub;
        }
    }
}
