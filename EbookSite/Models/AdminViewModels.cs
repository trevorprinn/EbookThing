using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EbookSite.Models {
    public class AdminViewModel {
        public bool GutenbergLoaderRunning { get; }

        public AdminViewModel() {
            GutenbergLoaderRunning = EbookSite.GutenbergLoadTask.Running;
        }
    }
}