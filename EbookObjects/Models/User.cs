using System;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    public partial class User
    {
        public User() { }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Identity { get; set; }
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
