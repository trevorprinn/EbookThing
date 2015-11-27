using System;
using System.Collections.Generic;
using System.Linq;
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
            _doc = XDocument.Load("EbookObjects.Secret.xml");
        }

        public static string GoogleBooksApiKey => _doc.Root.Element("GoogleBooksApiKey")?.Value;
    }
}
