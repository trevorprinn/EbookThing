using System;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    public partial class Publisher
    {
        public Publisher() { }

        public int PublisherId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
