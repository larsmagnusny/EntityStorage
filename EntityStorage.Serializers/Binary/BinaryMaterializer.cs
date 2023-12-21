using System.Linq.Expressions;
using System.Reflection;

namespace EntityStorage.Serializers.Binary
{
    public static class BinaryMaterializer
    {
        public static Dictionary<Type, MethodInfo> _typeWriters = new()
        {
            { typeof(string), typeof(BinaryGlobalWriters).GetMethod("WriteString") },
            { typeof(DateTime), typeof(BinaryGlobalWriters).GetMethod("WriteDateTime") },
            { typeof(Guid), typeof(BinaryGlobalWriters).GetMethod("WriteGuid") }
        };

        public static Dictionary<Type, MethodInfo> _typeReader = new()
        {
            { typeof(string), typeof(BinaryGlobalWriters).GetMethod("ReadString") },
            { typeof(DateTime), typeof(BinaryGlobalWriters).GetMethod("ReadDateTime") },
            { typeof(Guid), typeof(BinaryGlobalWriters).GetMethod("ReadGuid") }
        };

        public static Action<Stream, long, T> CreateWriter<T>()
        {
            var entityType = typeof(T);
            var fields = entityType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var streamVar = Expression.Parameter(typeof(Stream), "stream");
            var offsetVar = Expression.Parameter(typeof(long), "offset");
            var entityVar = Expression.Parameter(entityType, "entity");
            var genericWriteMethodInfo = typeof(BinaryGlobalWriters).GetMethod("Write");

            var writes = new List<Expression>();

            foreach (var property in properties)
            {
                MethodInfo? writeMethod;
                if (property.PropertyType.IsValueType)
                    writeMethod = genericWriteMethodInfo.MakeGenericMethod(property.PropertyType);
                else
                    writeMethod = _typeWriters[property.PropertyType];

                writes.Add(Expression.Call(null, writeMethod, streamVar, Expression.Property(entityVar, property)));
            }

            foreach (var field in fields)
            {
                MethodInfo? writeMethod;
                if (field.FieldType.IsValueType)
                    writeMethod = genericWriteMethodInfo.MakeGenericMethod(field.FieldType);
                else
                    writeMethod = _typeWriters[field.FieldType];

                writes.Add(Expression.Call(null, writeMethod, streamVar, Expression.Field(entityVar, field)));
            }

            var block = Expression.Block(writes);

            return Expression.Lambda<Action<Stream, long, T>>(block, streamVar, offsetVar, entityVar).Compile();
        }

        public static Func<Stream, long, T> CreateReader<T>()
        {
            var entityType = typeof(T);
            var fields = entityType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var streamVar = Expression.Parameter(typeof(Stream), "stream");
            var offsetVar = Expression.Parameter(typeof(long), "offset");
            var genericReadMethodInfo = typeof(BinaryGlobalWriters).GetMethod("Read");

            var retVar = Expression.Variable(entityType);
            var expressions = new List<Expression>()
            {
                Expression.Assign(retVar, Expression.New(entityType.GetConstructor(Array.Empty<Type>())))
            };

            foreach (var property in properties)
            {
                MethodInfo? readMethod;
                if (property.PropertyType.IsValueType)
                    readMethod = genericReadMethodInfo.MakeGenericMethod(property.PropertyType);
                else
                    readMethod = _typeReader[property.PropertyType];

                expressions.Add(Expression.Assign(Expression.Property(retVar, property), Expression.Call(null, readMethod, streamVar)));
            }

            foreach (var field in fields)
            {
                MethodInfo? readMethod;
                if (field.FieldType.IsValueType)
                    readMethod = genericReadMethodInfo.MakeGenericMethod(field.FieldType);
                else
                    readMethod = _typeReader[field.FieldType];

                expressions.Add(Expression.Assign(Expression.Field(retVar, field), Expression.Call(null, readMethod, streamVar)));
            }

            expressions.Add(retVar);

            var block = Expression.Block(new[] { retVar }, expressions);

            return Expression.Lambda<Func<Stream, long, T>>(block, streamVar, offsetVar).Compile();
        }
    }
}
