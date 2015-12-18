using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EbookObjects.Models;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace EbookSite.Models {

    public class DisplayBooksViewModel {

        public class DisplayBook {
            public int BookId { get; }
            public string Title { get; }
            public string Author { get; }
            public string Tags { get; }
            public DisplayBook(EbookObjects.Models.Book book) {
                BookId = book.BookId;
                Title = book.Title;
                Author = book.Author?.Name;
                Tags = string.Join(", ", book.Tags.Select(t => t.Item));
            }
        }

        public DisplayBook[] Books { get; private set; }

        public IEnumerable<Book> BookSet {
            set { Books = value.Select(b => new DisplayBook(b)).ToArray(); }
        }
    }

    public class BookViewModel {
        public string Title { get; }

        public int? Author { get; }

        public SelectList Authors { get; }

        public int BookId { get; }

        public int? Publisher { get; }

        public SelectList Publishers { get; }

        public string Description { get; }

        public List<Tag> Tags { get; }

        public string[] SelectedTags { get; }

        public string TagsInfo { get; }

        public MultiSelectList TagList { get; }

        public Dictionary<string, string> BookIdents { get; }

        public string[] Idents { get; }

        public BookViewModel(Book book, EbooksContext db) {
            BookId = book.BookId;
            Title = book.Title;
            Author = book.Author?.AuthorId;
            Authors = new SelectList(new Author[] { new Author() }.Concat(db.Authors.Where(a => a.Books.Any(b => b.UserId == book.UserId))).ToArray(), "AuthorId", "Name", Author);
            Publisher = book.Publisher?.PublisherId;
            Publishers = new SelectList(new Publisher[] { new Publisher() }.Concat(db.Publishers.Where(p => p.Books.Any(b => b.UserId == book.UserId))).ToArray(), "PublisherId", "Name", Publisher);
            Description = book.Description;
            SelectedTags = book.Tags.Select(t => t.Item).ToArray();

            // Because of the bug in ListBoxFor both the tag objects and a tag list are required to
            // populate the tagger control.
            Tags = book.Tags.ToList();
            TagList = new MultiSelectList(db.Tags.Where(t => t.Books.Any(b => b.UserId == book.UserId)).ToArray(), "Item", "Item", SelectedTags);

            // The JSON data required by the tagger control.
            var tagsInfo = new JObject();
            foreach (var tag in db.Tags) {
                var item = new JObject();
                item["id"] = tag.Item;
                item["selected"] = book.Tags.Contains(tag);
                item["suggestable"] = true;
                item["suggestion"] = tag.Item;
                item["key"] = tag.Item;
                tagsInfo.Add(tag.Item, item);
            }
            TagsInfo = tagsInfo.ToString();

            Idents = db.Idents.Where(i => i.BookIdents.Any(bi => bi.Book.UserId == book.UserId)).Select(i => i.Name).ToArray();
            BookIdents = book.BookIdents.ToDictionary(bi => bi.Ident.Name, bi => bi.Identifier);
        }
    }

    public class BookEditedViewModel {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int? AuthorId {
            get {
                int id;
                if (int.TryParse(Author, out id)) return id;
                return null;
            }
        }
        public string Publisher { get; set; }
        public int? PublisherId {
            get {
                int id;
                if (int.TryParse(Publisher, out id)) return id;
                return null;
            }
        }

        [AllowHtml]
        public string Description { get; set; }

        public string[] Tags { get; set; }

        public Dictionary<string, string> BookIdents { get; set; }
    }

    public class CoverViewModel {
        public int BookId { get; set; }

        public string Title { get; set; }
    }



}