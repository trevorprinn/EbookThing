using System;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    public partial class BookIdent
    {
        public int BookId { get; set; }
        public int IdentId { get; set; }
        public string Identifier { get; set; }
        public virtual Book Book { get; set; }
        public virtual Ident Ident { get; set; }
    }
}
