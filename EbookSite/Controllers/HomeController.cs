using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EbookSite.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            var name = User.Identity.GetUserId();
            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Contact Babbacombe Computers.";

            return View();
        }
    }
}