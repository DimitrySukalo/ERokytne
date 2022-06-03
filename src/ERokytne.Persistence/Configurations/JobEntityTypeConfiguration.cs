using ERokytne.Domain.Entities;
using ERokytne.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERokytne.Persistence.Configurations;

public class JobEntityTypeConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(e => e.Id);
        builder.ToTable("Jobs");

        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(50);
        builder.HasOne(e => e.TelegramUser).WithMany(e => e.Jobs)
            .HasForeignKey(e => e.TelegramUserId);
        builder.AddTrackEntityConfiguration();
    }
}