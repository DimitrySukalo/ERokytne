using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ERokytne.Persistence.ValueComparers
{
    public class DateTimeValueConverter : ValueConverter<DateTime, DateTime>
    {
        private static readonly Expression<Func<DateTime, DateTime>> ToProviderExpression = x => x;

        private static readonly Expression<Func<DateTime, DateTime>> FromProviderExpression =
            x => DateTime.SpecifyKind(x, DateTimeKind.Utc);

        public DateTimeValueConverter() : base(ToProviderExpression, FromProviderExpression)
        {
        }
    }
}