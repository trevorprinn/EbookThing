using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace EbookObjects.Models.Mapping
{
    public class IdentMap : EntityTypeConfiguration<Ident>
    {
        public IdentMap()
        {
            // Primary Key
            this.HasKey(t => t.IdentId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Ident");
            this.Property(t => t.IdentId).HasColumnName("IdentId");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
