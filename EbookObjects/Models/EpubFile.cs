using System;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    public partial class EpubFile
    {
        public EpubFile()
        {
            this.Books = new List<Book>();
        }

        public int FileId { get; set; }
        public byte[] Contents { get; set; }
        public string Checksum { get; set; }
        public virtual ICollection<Book> Books { get; set; }

        public int? GutBookId { get; set; }
        public virtual GutBook GutBook { get; set; }
        public bool? GutBookWithImages { get; set; }
    }
}
