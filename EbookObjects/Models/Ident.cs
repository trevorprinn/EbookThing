using System;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    public partial class Ident
    {
        public Ident() { }

        public int IdentId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<BookIdent> BookIdents { get; set; } = new List<BookIdent>();
    }
}
