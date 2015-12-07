using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EbookSite.Models;
using EbookObjects;
using EbookObjects.Models;

namespace EbookSite.Controllers
{
    [Authorize]
    public class EbooksController : Controller
    {
        // GET: Ebooks
        public ActionResult Index(string filter) {
            using (var db = new EbooksContext()) {
                var books = db.GetEbooksUser(User).Books.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(filter)) {
                    var usefilter = filter.ToLower();
                    books = books.Where(b => b.Title.ToLower().Contains(usefilter));
                }
                var model = new DisplayBooksViewModel { Filter = filter, BookSet = books };
                return View(model);
            }
        }

        public ActionResult Upload() {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles() {
            using (var db = new EbooksContext()) {
                var user = db.GetEbooksUser(User);
                foreach (string f in Request.Files) {
                    HttpPostedFileBase hpf = Request.Files[f] as HttpPostedFileBase;
                    if (hpf.ContentLength == 0) continue;
                    using (var m = new MemoryStream(hpf.ContentLength)) {
                        hpf.InputStream.CopyTo(m);
                        var epub = new Epub(m);
                        Book.Load(db, epub, user.UserId);
                        db.SaveChanges();
                    }
                }
            }
            return Redirect("~/Ebooks");
        }

        public ActionResult Edit(int? bookId) {
            if (bookId == null) return RedirectToAction("Index");
            using (var db = new EbooksContext()) {
                var user = db.GetEbooksUser(User);
                var book = db.Books.SingleOrDefault(b => b.BookId == bookId && b.UserId == user.UserId);
                if (book == null) return Redirect(Request.UrlReferrer.ToString());
                return View(new BookViewModel(book, db));
            }
        }

        [HttpPost]
        public ActionResult Edit(BookEditedViewModel model) {
            using (var db = new EbooksContext()) {
                var book = db.Books.Single(b => b.BookId == model.BookId);
                book.Title = model.Title;
                if (model.Author.StartsWith("~")) {
                    book.Author = Author.Get(db, model.Author.Substring(1));
                } else {
                    book.AuthorId = model.AuthorId != 0 ? model.AuthorId : null;
                }
                if (model.Publisher.StartsWith("~")) {
                    book.Publisher = Publisher.Get(db, model.Publisher.Substring(1));
                } else {
                    book.PublisherId = model.PublisherId != 0 ? model.PublisherId : null;
                }

                book.Description = model.Description;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Cover(int bookId) {
            using (var db = new EbooksContext()) {
                var user = db.GetEbooksUser(User);
                var book = db.Books.SingleOrDefault(b => b.BookId == bookId && b.UserId == user.UserId);
                if (book == null) return RedirectToAction("Index");
                var model = new CoverViewModel { BookId = book.BookId, Title = book.Title };
                return View(model);
            }
        }

        public ActionResult Download(int bookId) {
            using (var db = new EbooksContext()) {
                var user = db.GetEbooksUser(User);
                var book = db.Books.SingleOrDefault(b => b.BookId == bookId && b.UserId == user.UserId);
                if (book == null) return RedirectToAction("Index");
                return File(book.EpubFile.Contents, System.Net.Mime.MediaTypeNames.Application.Octet, book.Title + ".epub");
            }
        }

        public ActionResult Delete(int bookId) {
            using (var db = new EbooksContext()) {
                var user = db.GetEbooksUser(User);
                var book = db.Books.SingleOrDefault(b => b.BookId == bookId && b.UserId == user.UserId);
                if (book != null) {
                    db.Books.Remove(book);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
    }
}