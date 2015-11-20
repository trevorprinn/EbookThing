using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EbookSite {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ReadBookContent",
                url: "ReadBook/ReadContent/{bookId}/{path}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}",
                defaults: new { controller = "ReadBook", action = "ReadContent",
                    p1 = UrlParameter.Optional, p2 = UrlParameter.Optional, p3 = UrlParameter.Optional,
                    p4 = UrlParameter.Optional, p5 = UrlParameter.Optional, p6 = UrlParameter.Optional
                }
            );
        }
    }
}
