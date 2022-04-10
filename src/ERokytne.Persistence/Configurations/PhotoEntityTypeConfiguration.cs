using ERokytne.Domain.Entities;
using ERokytne.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERokytne.Persistence.Configurations;

public class PhotoEntityTypeConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.ToTable("Photos");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Path).HasMaxLength(500);
        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(50);
        
        builder.AddTrackEntityConfiguration();
    }
}