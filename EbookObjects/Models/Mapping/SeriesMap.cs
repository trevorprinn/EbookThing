using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace EbookObjects.Models.Mapping
{
    public class SeriesMap : EntityTypeConfiguration<Series>
    {
        public SeriesMap()
        {
            // Primary Key
            this.HasKey(t => t.SeriesId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Series");
            this.Property(t => t.SeriesId).HasColumnName("SeriesId");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
