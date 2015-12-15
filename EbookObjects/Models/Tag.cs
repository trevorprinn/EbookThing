using System;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    public partial class Tag
    {
        public Tag() { }

        public int TagId { get; set; }
        public string Item { get; set; }
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

        public override string ToString() {
            return Item;
        }
    }
}
