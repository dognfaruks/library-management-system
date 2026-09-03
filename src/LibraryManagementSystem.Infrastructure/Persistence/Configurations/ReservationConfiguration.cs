using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Status).HasConversion<short>();

        builder.HasIndex(r => r.BookId);
        builder.HasIndex(r => r.Status);

        // İş kuralı: Aynı kullanıcı aynı kitap için yalnızca bir AKTİF rezervasyon oluşturabilir
        // (Filtered / Partial Unique Index - sadece Status = Active olanlarda geçerli)
        builder.HasIndex(r => new { r.UserId, r.BookId })
            .HasFilter($"\"Status\" = {(int)ReservationStatus.Active}")
            .IsUnique();

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Book)
            .WithMany(b => b.Reservations)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}