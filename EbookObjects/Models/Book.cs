using System;
using System.Collections.Generic;

namespace EbookObjects.Models
{
    public partial class Book
    {
        public Book() { }

        public int BookId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public Nullable<int> AuthorId { get; set; }
        public Nullable<int> PublisherId { get; set; }
        public Nullable<int> CoverId { get; set; }
        public int FileId { get; set; }
        public Nullable<int> SeriesId { get; set; }
        public Nullable<decimal> SeriesNbr { get; set; }
        public string Description { get; set; }
        public virtual Author Author { get; set; }
        public virtual Cover Cover { get; set; }
        public virtual EpubFile EpubFile { get; set; }
        public virtual Publisher Publisher { get; set; }
        public virtual Series Series { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<BookIdent> BookIdents { get; set; } = new List<BookIdent>();
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
