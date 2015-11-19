using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EbookObjects.Models;
using System.Web.Mvc;

namespace EbookSite.Models {

    public class DisplayBooksViewModel {

        public class DisplayBook {
            public int BookId { get; }
            public string Title { get; }
            public string Author { get; }
            public DisplayBook(EbookObjects.Models.Book book) {
                BookId = book.BookId;
                Title = book.Title;
                Author = book.Author.Name;
            }
        }

        public DisplayBook[] Books { get; private set; }

        public IEnumerable<Book> BookSet {
            set { Books = value.Select(b => new DisplayBook(b)).ToArray(); }
        }

        [Display(Name = "Filter")]
        public string Filter { get; set; }
    }

    public class BookViewModel {
        public string Title { get; set; }

        public int? AuthorId { get; set; }

        public string Author { get; set; }

        public SelectList Authors { get; set; }

        public int BookId { get; }

        public BookViewModel(Book book, EbooksContext db) {
            BookId = book.BookId;
            Title = book.Title;
            AuthorId = book.AuthorId;
            Author = book.Author?.Name;
            Authors = new SelectList(db.Authors.Where(a => a.Books.Any(b => b.UserId == book.UserId)).ToArray(), "AuthorId", "Name", Author);
        }
    }

    public class BookEditedViewModel {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }

    public class CoverViewModel {
        public int BookId { get; set; }

        public string Title { get; set; }
    }



}