using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace EbookObjects.Models.Mapping
{
    public class EpubFileMap : EntityTypeConfiguration<EpubFile>
    {
        public EpubFileMap()
        {
            // Primary Key
            this.HasKey(t => t.FileId);

            // Properties
            this.Property(t => t.Contents)
                .IsRequired();

            this.Property(t => t.Checksum)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("EpubFile");
            this.Property(t => t.FileId).HasColumnName("FileId");
            this.Property(t => t.Contents).HasColumnName("Contents");
            this.Property(t => t.Checksum).HasColumnName("Checksum");
        }
    }
}
