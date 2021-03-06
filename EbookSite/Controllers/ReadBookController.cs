﻿using System;
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
    public class ReadBookController : Controller {
        // GET: ReadBook
        public ActionResult Index(int bookId = 0, int spineIndex = 0) {
            using (var db = new EbooksContext()) {
                var user = db.GetEbooksUser(User);
                var book = db.Books.SingleOrDefault(b => b.BookId == bookId && b.UserId == user.UserId);
                if (book == null) return RedirectToAction("Index", "Ebooks");
                using (var ep = new Epub(book)) {
                    var spine = ep.SpineRefs.ToArray();
                    spineIndex = Math.Max(0, Math.Min(spineIndex, spine.Length - 1));
                    string url = Url.Action($"ReadContent/{bookId}/{spine[spineIndex]}");

                    var model = new ReadBookViewModel {
                        SpineRefs = ep.SpineRefs.ToArray(),
                        Title = book.Title,
                        Url = url,
                        BookId = bookId,
                        Toc = ep.Toc
                    };

                    return View(model);
                }
            }
        }

        public ActionResult ReadContent() {
            int bookId = Convert.ToInt32(Request.Url.Segments[3].TrimEnd('/'));
            string file = HttpUtility.UrlDecode(string.Join("", Request.Url.Segments.Skip(4)));
            using (var db = new EbooksContext()) {
                var userId = db.GetEbooksUser(User).UserId;
                var book = db.Books.Single(b => b.BookId == bookId && b.UserId == userId);
                if (book == null) return null;
                using (var ep = new Epub(book)) {
                    return new FileStreamResult(ep.GetContentFile(file), "text/html");
                }
            }
        }
    }
}