using System;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    public partial class Author
    {
        public Author() { }

        public int AuthorId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
