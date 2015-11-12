using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace EbookObjects.Models.Mapping
{
    public class CoverMap : EntityTypeConfiguration<Cover>
    {
        public CoverMap()
        {
            // Primary Key
            this.HasKey(t => t.CoverId);

            // Properties
            this.Property(t => t.Picture)
                .IsRequired();

            this.Property(t => t.Thumbnail)
                .IsRequired();

            this.Property(t => t.Checksum)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Cover");
            this.Property(t => t.CoverId).HasColumnName("CoverId");
            this.Property(t => t.Picture).HasColumnName("Picture");
            this.Property(t => t.Thumbnail).HasColumnName("Thumbnail");
            this.Property(t => t.Checksum).HasColumnName("Checksum");
        }
    }
}
