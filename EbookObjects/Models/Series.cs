using System;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    public partial class Series
    {
        public Series()
        {
            this.Books = new List<Book>();
        }

        public int SeriesId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
