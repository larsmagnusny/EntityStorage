namespace EntityStorage.Serializers.Binary
{
    public static class BinaryStreamHelpers
    {
        public static Dictionary<Type, Delegate> _writersCache = new();
        public static Dictionary<Type, Delegate> _readersCache = new();

        public static Stream Append<T>(this Stream stream, T entity, long offset)
        {
            var entityType = typeof(T);
            if (!_writersCache.TryGetValue(entityType, out var func))
            {
                // Create materializers
            }

            if (func is null)
                throw new Exception("Materializer was null");

            ((Action<Stream, long, T>)func).Invoke(stream, offset, entity);

            return stream;
        }

        public static T? Read<T>(Stream stream, long offset = 0)
        {
            var entityType = typeof(T);
            if (!_readersCache.TryGetValue(entityType, out var func))
            {
                // Create materializers
            }

            if (func is null)
                throw new Exception("Materializer was null");

            return ((Func<Stream, long, T>)func).Invoke(stream, offset);
        }
    }
}
