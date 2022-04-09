using ERokytne.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERokytne.Persistence.Configurations;

public class AnnouncementEntityTypeConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("Announcements");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Text).HasMaxLength(5000);
        builder.HasMany(e => e.Photos).WithOne(e => e.Announcement)
            .HasForeignKey(e => e.AnnouncementId);
    }
}