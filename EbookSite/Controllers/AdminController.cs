using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EbookObjects;
using EbookObjects.Models;
using EbookSite;
using System.IO;
using System.Xml.Linq;

namespace EbookSite.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            using (var db = new EbooksContext()) {
                if (!db.GetEbooksUser(User).IsAdmin) return RedirectToAction("Index", "Ebooks");
            }
            return View();
        }

        [HttpPost]
        public ActionResult UploadGutCatalogue() {
            using (var db = new EbooksContext()) {
                if (!db.GetEbooksUser(User).IsAdmin) return RedirectToAction("Index", "Ebooks");
            }
            if (Request.Files.Count > 0) {
                HttpPostedFileBase hpf = Request.Files[0] as HttpPostedFileBase;
                if (hpf.ContentLength == 0) return View("Index");
                using (var s = hpf.InputStream) {
                    new GutCatalogueLoader().Load(s);
                }
                ViewBag.Uploaded = true;
            }
            return View("Index");
        }
    }
}