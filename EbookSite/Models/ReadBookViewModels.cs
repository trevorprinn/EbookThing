using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EbookSite.Models {
    public class ReadBookViewModel {
        public string[] SpineRefs;

        public string Title { get; set; }

        public string Url { get; set; }

        public int BookId { get; set; }

        public EbookObjects.Toc Toc;
    }
}