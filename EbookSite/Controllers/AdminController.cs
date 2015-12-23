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
using FluentScheduler;
using System.Threading;
using EbookSite.Models;

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
            return View(new AdminViewModel());
        }

        public ActionResult ImportGutCatalogue() {
            using (var db = new EbooksContext()) {
                if (!db.GetEbooksUser(User).IsAdmin) return RedirectToAction("Index", "Ebooks");
            }

            TaskManager.AddTask<GutenbergLoadTask>(schedule => schedule.ToRunNow());
            // Wait a second for it to start
            Thread.Sleep(1000);
            return RedirectToAction("Index");
        }
    }
}