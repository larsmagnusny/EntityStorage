namespace EntityStorage.Serializers.Binary
{
    public static class BinaryStreamHelpers
    {
        private static Dictionary<Type, Delegate> _writersCache = new();
        private static Dictionary<Type, Delegate> _readersCache = new();

        public static Stream Write<T>(this Stream stream, T entity, long offset)
        {
            var entityType = typeof(T);
            if (!_writersCache.TryGetValue(entityType, out var func))
            {
                func = BinaryMaterializer.CreateWriter<T>();
                _writersCache.Add(entityType, func);
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
                func = BinaryMaterializer.CreateReader<T>();
                _readersCache.Add(entityType, func);
            }

            if (func is null)
                throw new Exception("Materializer was null");

            return ((Func<Stream, long, T>)func).Invoke(stream, offset);
        }
    }
}
