using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Title).HasMaxLength(255).IsRequired();
        builder.Property(b => b.ISBN).HasMaxLength(20).IsRequired();
        builder.Property(b => b.Stock).IsRequired();

        // İş kuralı: Stok negatif olamaz (veritabanı seviyesinde garanti)
        builder.ToTable(t => t.HasCheckConstraint("CK_Books_Stock_NonNegative", "\"Stock\" >= 0"));

        builder.HasIndex(b => b.ISBN).IsUnique();
        builder.HasIndex(b => b.Title); // arama (search) performansı için

        builder.HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}