using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    internal static class CsvExtensions {
        public static IEnumerable<T> FromCsv<T>(this StreamReader input) {
            // Get the field names from the csv file
            var fnames = parseCsvRecord(input);
            if (fnames == null) yield break;
            var names = fnames.ToList();

            var props = typeof(T).GetProperties();
            string[] values;
            while (true) {
                values = parseCsvRecord(input);
                if (values == null) break;
                var o = (T)Activator.CreateInstance(typeof(T));
                foreach (var prop in props) {
                    var ix = names.IndexOf(prop.Name);
                    if (ix >= 0 && ix <= values.Length - 1) {
                        if (prop.PropertyType == typeof(string)) {
                            prop.SetValue(o, string.IsNullOrEmpty(values[ix]) ? null : values[ix]);
                        } else {
                            var conv = TypeDescriptor.GetConverter(prop.PropertyType);
                            if (conv.IsValid(values[ix])) {
                                prop.SetValue(o, conv.ConvertFrom(values[ix]));
                            }
                        }
                    }
                }
                yield return o;
            }
        }

        private static Regex _csvParser = new Regex("(?:^|,)(\\\"(?:[^\\\"]+|\\\"\\\")*\\\"|[^,]*)");

        private static string[] parseCsvRecord(StreamReader input) {
            string rowData = getCsvRowData(input);
            if (rowData == null) return null;

            List<string> fields = new List<string>();
            foreach (Match match in _csvParser.Matches(rowData)) {
                StringBuilder field = new StringBuilder(match.Groups[1].Value.Trim());
                if (field.ToString().StartsWith("\"") && field.ToString().EndsWith("\"")) {
                    field.Remove(0, 1);
                    field.Remove(field.Length - 1, 1);
                }
                field.Replace("\"\"", "\"");
                fields.Add(field.ToString());
            }
            return fields.ToArray();
        }

        private static Regex _quoteCounter = new Regex("\\\"");

        private static string getCsvRowData(StreamReader input) {
            int quoteCount = 0;
            StringBuilder s = new StringBuilder();
            string line = input.ReadLine();
            while (line != null) {
                quoteCount += _quoteCounter.Matches(line).Count;
                if (s.Length > 0) s.Append("\r\n");
                s.Append(line);
                if (quoteCount % 2 == 0) break;
                // We have an odd number of quotes, so there must be more.
                line = input.ReadLine();
            }
            if (line == null) return null;
            return s.ToString();
        }
    }
}
