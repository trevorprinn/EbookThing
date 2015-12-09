using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EbookObjects;
using EbookObjects.Models;
using EbookSite.Models;

namespace EbookSite.Controllers
{
    [Authorize]
    public class ReadBookController : Controller
    {
        // GET: ReadBook
        public ActionResult Index(int bookId = 0)
        {
            using (var db = new EbooksContext()) {
                var user = db.GetEbooksUser(User);
                var book = db.Books.SingleOrDefault(b => b.BookId == bookId && b.UserId == user.UserId);
                if (book == null) return RedirectToAction("Index", "Ebooks");
                using (var ep = new Epub(book)) {
                    var spine = ep.SpineRefs.ToArray();
                    string url = Url.Action($"ReadContent/{bookId}/{ep.RootFolder}");
                    if (!url.EndsWith("/")) url += '/';
                    var model = new ReadBookViewModel {
                        Title = book.Title,
                        Url = url,
                        BookId = bookId
                    };

                    return View(model);
                }
            }
        }

        public ActionResult ReadContent() {
            int bookId = Convert.ToInt32(Request.Url.Segments[3].TrimEnd('/'));
            string file = HttpUtility.UrlDecode(string.Join("", Request.Url.Segments.Skip(4)));
            using (var db = new EbooksContext()) {
                var book = db.Books.Single(b => b.BookId == bookId);
                using (var ep = new Epub(book)) {
                    if (string.IsNullOrWhiteSpace(file) || file == ep.RootFolder) {
                        return View("BookViewer", new BookViewerViewModel { Body = ep.Stitch() });
                    } else {
                        return new FileStreamResult(ep.GetContentFile(file, true), "text/html");
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult ChangePage(int bookId, int spineIndex) {
            using (var db = new EbooksContext()) {
                var user = db.GetEbooksUser(User);
                var book = db.Books.SingleOrDefault(b => b.BookId == bookId && b.UserId == user.UserId);
                if (book == null) return RedirectToAction("Index", "Ebooks");
                using (var ep = new Epub(book)) {
                    var spine = ep.SpineRefs.ToArray();
                    spineIndex = Math.Max(0, Math.Min(spineIndex, spine.Length - 1));
                    return Json(new {
                        url = Url.Action($"ReadContent/{bookId}/{spine[spineIndex]}"),
                        canPrevious = spineIndex > 0,
                        canNext = spineIndex < spine.Length - 1,
                        spineIndex = spineIndex,
                        bookId = bookId
                    });
                }
            }
        }
    }
}