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
    }
}