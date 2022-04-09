using ERokytne.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERokytne.Persistence.Configurations;

public class AdminEntityTypeConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.HasKey(e => e.Id);
        builder.ToTable("Admins");

        builder.Property(e => e.UserName).HasMaxLength(256);
        builder.Property(e => e.NormalizedEmail).HasMaxLength(256);
        builder.Property(e => e.EmailConfirmed).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.PasswordHash).HasMaxLength(2000);
        builder.Property(e => e.SecurityStamp).HasMaxLength(2000);
        builder.Property(e => e.ConcurrencyStamp).HasMaxLength(2000);
        builder.Property(e => e.PhoneNumber).HasMaxLength(50);
        builder.Property(e => e.PhoneNumberConfirmed).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.TwoFactorEnabled).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.LockoutEnabled).HasConversion<string>().HasMaxLength(10);
    }
}