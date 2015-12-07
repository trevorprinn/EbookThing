using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EbookObjects.Models;
using EbookSite.Models;
using EbookObjects;

namespace EbookSite.Controllers
{
    [Authorize]
    public class GutenbergController : Controller
    {
        // GET: Gutenberg
        public ActionResult Index(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return View(new GutenbergViewModel());
            using (var db = new EbooksContext()) {
                string usefilter = filter.ToLower();
                var books = db.GutBooks.Where(b => b.Title.ToLower().Contains(usefilter) || (b.GutAuthor != null && b.GutAuthor.Name.Contains(usefilter)))
                    .Take(50).OrderBy(b => b.Title).ThenBy(b => b.GutAuthor == null ? null : b.GutAuthor.Name);
                var vm = new GutenbergViewModel(books, filter);
                return View(vm);
            }
        }

        public ActionResult Cover(int bookId) {
            using (var db = new EbooksContext()) {
                var book = db.GutBooks.Single(b => b.GutBookId == bookId);
                return View(new CoverViewModel { BookId = bookId, Title = book.Title });
            }
        }

        public ActionResult Download(int gutBookId, bool images) {
            using (var db = new EbooksContext()) {
                var gutBook = db.GutBooks.Single(b => b.GutBookId == gutBookId);
                // Check whether the book has already been downloaded
                var ef = db.EpubFiles.SingleOrDefault(e => e.GutBookId == gutBook.GutBookId && e.GutBookWithImages == images);
                Epub ep;
                if (ef != null) {
                    ep = new Epub(ef.Contents);
                } else {
                    ep = new Epub(images ? gutBook.GetEpubImages() : gutBook.GetEpubNoImages());
                }
                try {
                    var user = db.GetEbooksUser(User);
                    var book = Book.Load(db, ep, user.UserId);
                    if (book.Publisher == null) book.Publisher = Publisher.Get(db, "Project Gutenberg");
                    if (ef == null) {
                        book.EpubFile.GutBookId = gutBook.GutBookId;
                        book.EpubFile.GutBookWithImages = images;
                    }
                    db.SaveChanges();
                } finally {
                    ep.Dispose();
                }
            }
            return RedirectToAction("Index", "Ebooks");
        }
    }
}