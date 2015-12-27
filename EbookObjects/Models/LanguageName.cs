using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    [Table("LanguageName")]
    public class LanguageName {

        [Key]
        public int LanguageNameId { get; set; }

        [MaxLength(100), Index(IsClustered = false, IsUnique = true)]
        public string Name { get; set; }

        public virtual ICollection<LanguageCode> LanguageCodes { get; set; }

        /// <summary>
        /// Gets the languages that are in use by Project Gutenberg, ordered by language
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static IEnumerable<string> InUse(EbooksContext db) {
            return db.GutBooks.Select(gb => gb.LanguageCode).SelectMany(lc => lc.LanguageNames).Select(ln => ln.Name).Distinct().OrderBy(ln => ln);
        }
    }
}
