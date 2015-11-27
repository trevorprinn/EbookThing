using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EbookObjects {
    /// <summary>
    /// Configuration information that shouldn't be stored with the source on github.
    /// </summary>
    internal static class SecretConfig {
        /*
         *  <config>
         *    <GoogleBooksApiKey>Whatever</GoogleBooksApiKey>
         *  </config>
         */
        private static XDocument _doc;

        static SecretConfig() {
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("EbookObjects.EbookObjects.Secret.xml")) {
                _doc = XDocument.Load(s);
            }
        }

        public static string GoogleBooksApiKey => _doc.Root.Element("GoogleBooksApiKey")?.Value;
    }
}
