using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EbookObjects.Models.Mapping
{
    public class BookIdentMap : EntityTypeConfiguration<BookIdent>
    {
        public BookIdentMap()
        {
            // Primary Key
            this.HasKey(t => new { t.BookId, t.IdentId });

            // Properties
            this.Property(t => t.BookId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IdentId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Identifier)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("BookIdent");
            this.Property(t => t.BookId).HasColumnName("BookId");
            this.Property(t => t.IdentId).HasColumnName("IdentId");
            this.Property(t => t.Identifier).HasColumnName("Identifier");

            // Relationships
            this.HasRequired(t => t.Book)
                .WithMany(t => t.BookIdents)
                .HasForeignKey(d => d.BookId);
            this.HasRequired(t => t.Ident)
                .WithMany(t => t.BookIdents)
                .HasForeignKey(d => d.IdentId);

        }
    }
}
