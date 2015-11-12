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
        public void SetTags(EbooksContext context, IEnumerable<string> newTags) {
            // Add the tags that aren't already against the book
            foreach (var tag in newTags.Except(Tags.Select(t => t.Item)).ToArray()) {
                var rtag = context.Tags.SingleOrDefault(t => t.Item == tag);
                if (rtag == null) {
                    context.Tags.Add(rtag = new Tag { Item = tag });
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
        /// <param name="context"></param>
        /// <param name="newIds">Item1 = the ident name, Item2 = the value for this book</param>
        public void SetIdentifiers(EbooksContext context, IEnumerable<Tuple<string, string>> newIds) {
            // Clear the idents against the book and re-add them.
            BookIdents.Clear();

            foreach (var id in newIds) {
                var ident = context.Idents.SingleOrDefault(i => i.Name == id.Item1);
                if (ident == null) {
                    context.Idents.Add(ident = new Ident { Name = id.Item1 });
                }
                context.BookIdents.Add(new BookIdent { Book = this, Ident = ident, Identifier = id.Item2 });
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
        /// <param name="context"></param>
        /// <param name="ep"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <remarks>Does not save the changes.</remarks>
        public static Book Load(EbooksContext context, Epub ep, int userId) {
            var book = new Book {
                UserId = userId,
                Title = ep.Title,
                Author = Author.Get(context, ep),
                Publisher = Publisher.Get(context, ep),
                Cover = Cover.Get(context, ep),
                EpubFile = EpubFile.Get(context, ep),
                Series = Series.Get(context, ep),
                SeriesNbr = ep.SeriesNbr,
                Description = ep.Description
            };
            book.SetTags(context, ep.Tags);
            book.SetIdentifiers(context, ep.Identifiers);

            context.Books.Add(book);

            return book;
        }

        /// <summary>
        /// Reloads the database record with the book data, overwriting any changes made by the user.
        /// </summary>
        /// <param name="context"></param>
        /// <remarks>Does not save the changes.</remarks>
        public void Reload(EbooksContext context) {
            var ep = new Epub(this);

            Title = ep.Title;
            Author = Author.Get(context, ep);
            Publisher = Publisher.Get(context, ep);
            Cover = Cover.Get(context, ep);
            EpubFile = EpubFile.Get(context, ep);
            Series = Series.Get(context, ep);
            SeriesNbr = ep.SeriesNbr;
            Description = ep.Description;
            SetTags(context, ep.Tags);
            SetIdentifiers(context, ep.Identifiers);
        }
    }
}
