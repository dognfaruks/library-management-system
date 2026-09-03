using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Infrastructure.Persistence.Configurations;

public class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
{
    public void Configure(EntityTypeBuilder<Borrowing> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Status).HasConversion<short>();

        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.BookId);
        builder.HasIndex(b => b.Status);

        builder.HasOne(b => b.User)
            .WithMany(u => u.Borrowings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Book)
            .WithMany(bk => bk.Borrowings)
            .HasForeignKey(b => b.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}