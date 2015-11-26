using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EbookObjects.Models;

namespace EbookSite.Models {
    public class GutenbergViewModel {
        public class Book {
            public int Id { get; }
            public string Author { get; }
            public string Title { get; }
            public string ImagesLink { get; }
            public string NoImagesLink { get; }

            public Book(GutBook book) {
                Id = book.GutBookId;
                Author = book.GutAuthor?.Name;
                Title = book.Title;
                ImagesLink = book.EpubUrlImages;
                NoImagesLink = book.EpubUrlNoImages;
            }
        }

        public string Filter { get; }

        public IEnumerable<Book> Books { get; }

        public GutenbergViewModel() {
            Books = new Book[0];
            Filter = null;
        }

        public GutenbergViewModel(IEnumerable<GutBook> books, string filter) {
            Books = books.Select(b => new Book(b)).ToArray();

            Filter = filter;
        }
    }
}