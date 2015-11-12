using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EbookObjects {
    static class XDocument11 {
        public static XDocument Load(string file) {
            using (var s = new FileStream(file, FileMode.Open, FileAccess.Read)) {
                return Load(s);
            }
        }

        public static XDocument Load(Stream s) {
            using (var r = new StreamReader(s)) {
                string xml = r.ReadToEnd();
                return XDocument.Parse(Regex.Replace(xml, @"(<\?xml.*)(version=\""1.1\"")(.*\?>)", (m) => {
                    return m.Groups[1] + "version=\"1.0\"" + m.Groups[3];
                }));
            }
        }
    }

    public static class SetExtensions {
        public static bool In<T>(this T @value, params T[] values) {
            return values.Contains(@value);
        }

        public static bool In<T>(this T @value, IQueryable<T> values) {
            return values.Contains(@value);
        }

        public static bool NotIn<T>(this T @value, params T[] values) {
            return !values.Contains(@value);
        }

        public static bool NotIn<T>(this T @value, IQueryable<T> values) {
            return !values.Contains(@value);
        }
    }

    internal static class ChecksumExtensions {
        public static string GetChecksum(this Stream s) {
            s.Seek(0, SeekOrigin.Begin);
            using (var hasher = SHA256.Create()) {
                var hash = hasher.ComputeHash(s);
                return string.Join(null, hash.Select(b => b.ToString("X2")));
            }
        }

        public static string GetChecksum(this byte[] data) {
            using (var hasher = SHA256.Create()) {
                var hash = hasher.ComputeHash(data);
                return string.Join(null, hash.Select(b => b.ToString("X2")));
            }
        }
    }

}
