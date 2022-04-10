using ERokytne.Domain.Entities;
using ERokytne.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERokytne.Persistence.Configurations;

public class SupportMessageEntityTypeConfiguration : IEntityTypeConfiguration<SupportMessage>
{
    public void Configure(EntityTypeBuilder<SupportMessage> builder)
    {
        builder.HasKey(e => e.Id);
        builder.ToTable("SupportMessages");

        builder.Property(e => e.Text).HasMaxLength(5000);
        builder.HasOne(e => e.TelegramUser).WithMany(e => e.SupportMessages)
            .HasForeignKey(e => e.TelegramUserId);
        
        builder.AddTrackEntityConfiguration();
    }
}