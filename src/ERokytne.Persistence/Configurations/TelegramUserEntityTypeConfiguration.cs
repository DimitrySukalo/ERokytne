using ERokytne.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERokytne.Persistence.Configurations;

public class TelegramUserEntityTypeConfiguration : IEntityTypeConfiguration<TelegramUser>
{
    public void Configure(EntityTypeBuilder<TelegramUser> builder)
    {
        builder.HasKey(e => e.Id);
        builder.ToTable("TelegramUsers");

        builder.Property(e => e.FullName).HasMaxLength(200);
        builder.Property(e => e.NickName).HasMaxLength(200);
        builder.Property(e => e.ChatId).HasMaxLength(100);
        builder.Property(e => e.PhoneNumber).HasMaxLength(30);

        builder.HasMany(e => e.Announcements).WithOne(e => e.TelegramUser)
            .HasForeignKey(e => e.TelegramUserId);
    }
}