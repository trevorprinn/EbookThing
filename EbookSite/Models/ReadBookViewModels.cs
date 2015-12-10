using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EbookSite.Models {
    public class ReadBookViewModel {
        public bool CanPrevious { get; set; }

        public bool CanNext { get; set; }

        public int SpineIndex { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public int BookId { get; set; }

        public EbookObjects.Toc Toc;
    }
}