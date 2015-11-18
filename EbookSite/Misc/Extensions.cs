using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
using EbookObjects.Models;

namespace EbookSite {
    public static class UserExtensions {
        public static User GetEbooksUser(this EbooksContext db, IPrincipal principal) {
            string aspnetId = principal.Identity.GetUserId();
            return db.Users.Single(u => u.Identity == aspnetId);
        }
    }
}