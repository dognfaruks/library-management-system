using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Rating).IsRequired();

        // İş kuralı: Puan 1-5 arasında olmalı
        builder.ToTable(t => t.HasCheckConstraint("CK_Reviews_Rating_Range", "\"Rating\" BETWEEN 1 AND 5"));

        // İş kuralı: Bir kullanıcı bir kitaba yalnızca bir kez yorum yapabilir
        builder.HasIndex(r => new { r.UserId, r.BookId }).IsUnique();
        builder.HasIndex(r => r.BookId);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}