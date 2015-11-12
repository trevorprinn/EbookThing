using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace EbookObjects.Models.Mapping
{
    public class BookMap : EntityTypeConfiguration<Book>
    {
        public BookMap()
        {
            // Primary Key
            this.HasKey(t => t.BookId);

            // Properties
            this.Property(t => t.Title)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Book");
            this.Property(t => t.BookId).HasColumnName("BookId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.AuthorId).HasColumnName("AuthorId");
            this.Property(t => t.PublisherId).HasColumnName("PublisherId");
            this.Property(t => t.CoverId).HasColumnName("CoverId");
            this.Property(t => t.FileId).HasColumnName("FileId");
            this.Property(t => t.SeriesId).HasColumnName("SeriesId");
            this.Property(t => t.SeriesNbr).HasColumnName("SeriesNbr");
            this.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            this.HasMany(t => t.Tags)
                .WithMany(t => t.Books)
                .Map(m =>
                    {
                        m.ToTable("BookTag");
                        m.MapLeftKey("BookId");
                        m.MapRightKey("TagId");
                    });

            this.HasOptional(t => t.Author)
                .WithMany(t => t.Books)
                .HasForeignKey(d => d.AuthorId);
            this.HasOptional(t => t.Cover)
                .WithMany(t => t.Books)
                .HasForeignKey(d => d.CoverId);
            this.HasRequired(t => t.EpubFile)
                .WithMany(t => t.Books)
                .HasForeignKey(d => d.FileId);
            this.HasOptional(t => t.Publisher)
                .WithMany(t => t.Books)
                .HasForeignKey(d => d.PublisherId);
            this.HasOptional(t => t.Series)
                .WithMany(t => t.Books)
                .HasForeignKey(d => d.SeriesId);
            this.HasRequired(t => t.User)
                .WithMany(t => t.Books)
                .HasForeignKey(d => d.UserId);

        }
    }
}
