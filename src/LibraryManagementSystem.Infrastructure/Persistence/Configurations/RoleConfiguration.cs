using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).HasMaxLength(20).IsRequired();
        builder.HasIndex(r => r.Name).IsUnique();

        // Uygulama açılışında USER ve ADMIN rolleri veritabanında hazır bulunsun
        builder.HasData(
            new Role { Id = 1, Name = "USER" },
            new Role { Id = 2, Name = "ADMIN" }
        );
    }
}