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
        public ActionResult Index() {
            using (var db = new EbooksContext()) {
                var model = new DisplayBooksViewModel { BookSet = db.GetEbooksUser(User).Books };
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
                var model = new BookViewModel(book, db);
                // ListBoxFor is broken, see https://social.msdn.microsoft.com/Forums/vstudio/en-US/05ee3b35-f3d3-48b4-83f5-ca3d9073624e/mvc-htmlhelper-listboxfor-and-listbox-multiselectlist-bug?forum=netfxbcl
                ViewBag.Tags = model.TagList;
                return View(model);
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
                book.SetTags(db, model.Tags);
                book.SetIdentifiers(db, model.BookIdents);

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

        public ActionResult Reload(int bookId) {
            using (var db = new EbooksContext()) {
                var user = db.GetEbooksUser(User);
                var book = db.Books.SingleOrDefault(b => b.BookId == bookId && b.UserId == user.UserId);
                if (book != null) {
                    book.Reload(db);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
    }
}