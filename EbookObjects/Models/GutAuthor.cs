using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    public class GutAuthor {
        [Key]
        public string GutAuthorId { get; set; }

        [Required, StringLength(250), Index("IX_GutAuthor_Name", IsClustered = false, IsUnique = true)]
        public string Name { get; set; }

        public virtual ICollection<GutBook> GutBooks { get; set; }

        public static GutAuthor Get(EbooksContext db, string name) {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var author = db.GutAuthors.SingleOrDefault(a => a.Name == name);
            if (author == null) {
                db.GutAuthors.Add(author = new GutAuthor { Name = name });
            }
            return author;
        }
    }
}
