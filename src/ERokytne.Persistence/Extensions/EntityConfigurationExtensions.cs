using ERokytne.Domain.Contracts;
using ERokytne.Persistence.ValueComparers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERokytne.Persistence.Extensions;

public static class EntityConfigurationExtensions
{
    public static void AddTrackEntityConfiguration<T>(this EntityTypeBuilder<T> builder)
        where T : class, ITrackEntity
    {
        builder.Property(e => e.CreatedOn).HasConversion(new DateTimeValueConverter());
        builder.Property(e => e.UpdatedOn).HasConversion(new NullableDateTimeValueConverter());
    }
}