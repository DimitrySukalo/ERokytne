using System.Text.Json;
using ERokytne.Domain.Entities;
using ERokytne.Persistence.Extensions;
using ERokytne.Persistence.ValueComparers;
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
        builder.HasOne(e => e.Group).WithMany(e => e.Announcements)
            .HasForeignKey(e => e.GroupId);
        builder.Property(e => e.Payload)
            .HasConversion(
                e => JsonSerializer.Serialize(e, new JsonSerializerOptions()),
                e => JsonSerializer.Deserialize<List<int>>(e, new JsonSerializerOptions()),
                new JsonValueComparer<List<int>>());
        
        builder.AddTrackEntityConfiguration();
    }
}