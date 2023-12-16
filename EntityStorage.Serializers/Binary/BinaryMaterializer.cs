using System.Linq.Expressions;

namespace EntityStorage.Serializers.Binary
{
    public static class BinaryMaterializer
    {
        public static Action<Stream, long, T> CreateWriter<T>()
        {
            var entityType = typeof(T);

            return Expression.Lambda<Action<Stream, long, T>>(Expression.Empty()).Compile();
        }

        public static Func<Stream, long, T> CreateReader<T>()
        {
            var entityType = typeof(T);

            return Expression.Lambda<Func<Stream, long, T>>(Expression.Empty()).Compile();
        }
    }
}
