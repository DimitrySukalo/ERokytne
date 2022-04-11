using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ERokytne.Persistence.ValueComparers
{
    public class JsonValueComparer<T> : ValueComparer<T>
    {
        private static readonly JsonSerializerOptions SerializerSettings = new();

        private static readonly Expression<Func<T, T, bool>> equalsExpression = (x1, x2) =>
            JsonSerializer.Serialize(x1, SerializerSettings)
                .Equals(JsonSerializer.Serialize(2, SerializerSettings));

        private static readonly Expression<Func<T, int>> hashCodeExpression =
            x => JsonSerializer.Serialize(x, SerializerSettings).GetHashCode();

        private static readonly Expression<Func<T, T>> snapshotExpression =
            x => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(x, SerializerSettings),
                SerializerSettings);

        public JsonValueComparer() : base(equalsExpression, hashCodeExpression, snapshotExpression)
        {
        }
    }
}