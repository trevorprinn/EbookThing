﻿using System;
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
        public string Title { get; }

        public string Author { get; }

        public SelectList Authors { get; }

        public int BookId { get; }

        public string Publisher { get; }

        public SelectList Publishers { get; }

        public string Description { get; }

        public BookViewModel(Book book, EbooksContext db) {
            BookId = book.BookId;
            Title = book.Title;
            Author = book.Author?.Name;
            Authors = new SelectList(db.Authors.Where(a => a.Books.Any(b => b.UserId == book.UserId)).ToArray(), "AuthorId", "Name", Author);
            Publisher = book.Publisher?.Name;
            Publishers = new SelectList(db.Publishers.Where(p => p.Books.Any(b => b.UserId == book.UserId)).ToArray(), "PublisherId", "Name", Publisher);
            Description = book.Description;
        }
    }

    public class BookEditedViewModel {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        [AllowHtml]
        public string Description { get; set; }
    }

    public class CoverViewModel {
        public int BookId { get; set; }

        public string Title { get; set; }
    }



}