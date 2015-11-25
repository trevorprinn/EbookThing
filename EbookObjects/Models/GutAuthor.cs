using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    [Table("GutAuthor")]
    public class GutAuthor {
        [Key]
        public int GutAuthorId { get; set; }

        [Required, StringLength(250), Index("IX_GutAuthor_Name", IsClustered = false, IsUnique = true)]
        public string Name { get; set; }

        public virtual ICollection<GutBook> GutBooks { get; set; }
    }
}
