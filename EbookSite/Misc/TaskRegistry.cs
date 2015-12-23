using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EbookSite {
    public class TaskRegistry : Registry {
        public TaskRegistry() {
            Schedule<GutenbergLoadTask>().ToRunEvery(1).Days().At(2, 30);
        }
    }
}