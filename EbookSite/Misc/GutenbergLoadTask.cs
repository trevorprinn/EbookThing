using EbookObjects;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace EbookSite {
    public class GutenbergLoadTask : ITask, IRegisteredObject {
        private bool _shuttingDown;
        private readonly object _lock = new object();

        public static bool Running { get; private set; }

        public GutenbergLoadTask() {
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute() {
            if (Running) return;
            lock (_lock) {
                if (_shuttingDown) return;
                Running = true;
                try {
                    var downloader = new GutCatalogueDownloader();
                    downloader.Run();
                } finally {
                    Running = false;
                }
            }
        }

        public void Stop(bool immediate) {
            lock (_lock) {
                _shuttingDown = true;
            }
            HostingEnvironment.UnregisterObject(this);
        }
    }
}