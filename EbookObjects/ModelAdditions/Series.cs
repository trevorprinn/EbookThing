using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    partial class Series {

        public static Series Get(EbooksContext context, Epub ep) {
            if (string.IsNullOrWhiteSpace(ep.Series)) return null;
            var series = context.Series.SingleOrDefault(s => s.Name == ep.Series);
            if (series == null) {
                context.Series.Add(series = new Series { Name = ep.Series });
            }
            return series;
        }
    }
}
