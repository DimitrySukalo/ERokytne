using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ERokytne.Persistence.ValueComparers
{
    public class NullableDateTimeValueConverter : ValueConverter<DateTime?, DateTime?>
    {
        private static readonly Expression<Func<DateTime?, DateTime?>> ToProviderExpression = x => x;

        private static readonly Expression<Func<DateTime?, DateTime?>> FromProviderExpression =
            x => x.HasValue ? DateTime.SpecifyKind(x.Value, DateTimeKind.Utc) : null;

        public NullableDateTimeValueConverter() : base(ToProviderExpression, FromProviderExpression)
        {
        }
    }
}