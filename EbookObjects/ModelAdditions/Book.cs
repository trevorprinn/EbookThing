using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    partial class Book {

        /// <summary>
        /// Sets the tags on the book to match the list passed in
        /// </summary>
        /// <param name="newTags"></param>
        public void SetTags(EbooksContext db, IEnumerable<string> newTags) {
            // Add the tags that aren't already against the book
            foreach (var tag in newTags.Except(Tags.Select(t => t.Item)).ToArray()) {
                var rtag = db.Tags.SingleOrDefault(t => t.Item == tag);
                if (rtag == null) {
                    db.Tags.Add(rtag = new Tag { Item = tag });
                }
                Tags.Add(rtag);
            }

            // Remove the tags that are no longer against the book
            foreach (var tag in Tags.Where(t => t.Item.NotIn(newTags.ToArray())).ToArray()) {
                Tags.Remove(tag);
            }
        }

        /// <summary>
        /// Sets the identifiers on the book to match the list passed in
        /// </summary>
        /// <param name="db"></param>
        /// <param name="newIds">Item1 = the ident name, Item2 = the value for this book</param>
        public void SetIdentifiers(EbooksContext db, IEnumerable<Tuple<string, string>> newIds) {
            // Clear the idents against the book and re-add them.
            BookIdents.Clear();

            foreach (var id in newIds) {
                var ident = db.Idents.SingleOrDefault(i => i.Name == id.Item1);
                if (ident == null) {
                    db.Idents.Add(ident = new Ident { Name = id.Item1 });
                }
                db.BookIdents.Add(new BookIdent { Book = this, Ident = ident, Identifier = id.Item2 });
            }
        }

        /// <summary>
        /// Gets the Table of Contents of the book.
        /// </summary>
        public Toc Toc {
            get {
                using (var b = new Epub(this)) return b.Toc;
            }
        }

        /// <summary>
        /// Loads a new book into the database for the given user, reusing records wherever possible.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ep"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <remarks>Does not save the changes.</remarks>
        public static Book Load(EbooksContext db, Epub ep, int userId) {
            var book = new Book {
                UserId = userId,
                Title = ep.Title,
                Author = Author.Get(db, ep),
                Publisher = Publisher.Get(db, ep),
                Cover = Cover.Get(db, ep),
                EpubFile = EpubFile.Get(db, ep),
                Series = Series.Get(db, ep),
                SeriesNbr = ep.SeriesNbr,
                Description = ep.Description
            };
            book.SetTags(db, ep.Tags);
            book.SetIdentifiers(db, ep.Identifiers);

            db.Books.Add(book);

            return book;
        }

        /// <summary>
        /// Reloads the database record with the book data, overwriting any changes made by the user.
        /// </summary>
        /// <param name="db"></param>
        /// <remarks>Does not save the changes.</remarks>
        public void Reload(EbooksContext db) {
            var ep = new Epub(this);

            Title = ep.Title;
            Author = Author.Get(db, ep);
            Publisher = Publisher.Get(db, ep);
            Cover = Cover.Get(db, ep);
            EpubFile = EpubFile.Get(db, ep);
            Series = Series.Get(db, ep);
            SeriesNbr = ep.SeriesNbr;
            Description = ep.Description;
            SetTags(db, ep.Tags);
            SetIdentifiers(db, ep.Identifiers);
        }
    }
}
