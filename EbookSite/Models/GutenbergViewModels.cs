using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EbookObjects;
using EbookObjects.Models;
using System.Web.Mvc;

namespace EbookSite.Models {
    public class GutenbergViewModel {
        public class Book {
            public int Id { get; }
            public string Author { get; }
            public string Title { get; }
            public bool HasImagesEpub { get; }
            public bool HasNoImagesEpub { get; }
            public string Languages { get; }

            public Book(GutBook book) {
                Id = book.GutBookId;
                Author = book.GutAuthor?.Name;
                Title = book.Title;
                HasImagesEpub = book.StandardEpubUrlImages || !string.IsNullOrWhiteSpace(book.EpubUrlImages);
                HasNoImagesEpub = book.StandardEpubUrlNoImages || !string.IsNullOrWhiteSpace(book.EpubUrlNoImages);
                Languages = string.Join("\r\n", book.GetLanguages());
            }
        }

        public string Filter { get; }

        public int Language { get; }

        public SelectList Languages { get; private set; }

        public IEnumerable<Book> Books { get; }

        private void setLanguages(IEnumerable<LanguageName> languages, int language = 0) {
            var langs = languages.ToList();
            langs.Insert(0, new LanguageName { LanguageNameId = 0, Name = "Any Language" });
            Languages = new SelectList(langs, "LanguageNameId", "Name", language);
        }

        public GutenbergViewModel(IEnumerable<LanguageName> languages) {
            Books = new Book[0];
            Filter = null;
            setLanguages(languages);
        }

        public GutenbergViewModel(IEnumerable<GutBook> books, string filter, int language, IEnumerable<LanguageName> languages) {
            Books = books.ToList().Select(b => new Book(b)).ToArray();

            Filter = filter;
            Language = language;
            setLanguages(languages, language);
        }
    }

    /// <summary>
    /// For displaying data about a Gutenberg book from Google Books.
    /// </summary>
    public class GutGoogleViewModel {
        public int GutBookId { get; }

        public string Title { get; }

        public string Author { get; }

        public IEnumerable<Details> Books { get; }

        public class Details {
            public string Title { get; }
            public string Authors { get; }

            public string Description { get; }

            public string ThumbnailUrl { get; }

            public string BookPageUrl { get; }

            public Details(GoogleBook book) {
                Title = book.Title;
                Authors = book.Authors == null ? "" : string.Join(", ", book.Authors);
                Description = book.Description;
                ThumbnailUrl = book.ThumbnailUrl;
                BookPageUrl = book.InfoLink;
            }
        }

        public GutGoogleViewModel(GutBook book, IEnumerable<GoogleBook> gbooks) {
            GutBookId = book.GutBookId;
            Title = book.Title;
            Author = book.GutAuthor.Name;

            Books = gbooks.Select(b => new Details(b)).ToArray();
        }
    }

}