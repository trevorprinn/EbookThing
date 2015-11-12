using System;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    public partial class User
    {
        public User()
        {
            this.Books = new List<Book>();
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
