using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    [Table("LanguageCode")]
    public class LanguageCode {
        [Key, MaxLength(3)]
        public string Code { get; set; }

        public virtual ICollection<LanguageName> LanguageNames { get; set; }

        public virtual ICollection<GutBook> GutBooks { get; set; }

    }
}
