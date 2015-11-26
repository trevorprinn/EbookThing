using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EbookObjects.Models;

namespace EbookSite.Controllers
{
    public class BookImageController : Controller
    {
        // GET: Image
        public ActionResult ShowThumbnail(int bookId)
        {
            using (var db = new EbooksContext()) {
                var thumbnail = db.Books.Single(b => b.BookId == bookId).Cover?.Thumbnail ?? Cover.EmptyThumbnail;
                return new FileStreamResult(new MemoryStream(thumbnail), "image/jpeg");
            }
        }

        public ActionResult ShowCover(int bookId) {
            using (var db = new EbooksContext()) {
                var cover = db.Books.Single(b => b.BookId == bookId).Cover;
                var pic = cover?.Picture ?? Cover.EmptyCover;
                var type = cover?.ContentType ?? "image/jpeg";
                return new FileStreamResult(new MemoryStream(pic), type);
            }
        }

        public ActionResult ShowGutThumbnail(int bookId) {
            using (var db = new EbooksContext()) {
                var thumbnail = db.GutBooks.Single(b => b.GutBookId == bookId).GetThumbnailData() ?? Cover.EmptyThumbnail;
                db.SaveChanges();   // May have read and cached the thumbnail data
                return new FileStreamResult(new MemoryStream(thumbnail), "image/jpeg");
            }
        }

        public ActionResult ShowGutCover(int bookId) {
            using (var db = new EbooksContext()) {
                var cover = db.GutBooks.Single(b => b.GutBookId == bookId).GetCoverData() ?? Cover.EmptyCover;
                return new FileStreamResult(new MemoryStream(cover), "image/jpeg");
            }
        }
    }
}