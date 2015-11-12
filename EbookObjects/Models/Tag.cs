using System;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    public partial class Tag
    {
        public Tag()
        {
            this.Books = new List<Book>();
        }

        public int TagId { get; set; }
        public string Item { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
