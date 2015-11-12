using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace EbookObjects.Models.Mapping
{
    public class PublisherMap : EntityTypeConfiguration<Publisher>
    {
        public PublisherMap()
        {
            // Primary Key
            this.HasKey(t => t.PublisherId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Publisher");
            this.Property(t => t.PublisherId).HasColumnName("PublisherId");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
