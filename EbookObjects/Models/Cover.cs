using System;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    public partial class Cover
    {
        public Cover()
        {
            this.Books = new List<Book>();
        }

        public int CoverId { get; set; }
        public byte[] Picture { get; set; }
        public byte[] Thumbnail { get; set; }
        public string Checksum { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
