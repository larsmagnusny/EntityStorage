using EntityStorage.Abstractions;
using EntityStorage.Attributes;
using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityStorage.Collections
{
    public class InMemoryEntityCollection<TEntity> : IEntityCollection<TEntity>
    {
        private readonly string[] _keyMemberNames;
        private readonly Func<TEntity, object> _keyProvider;
        private readonly Action<TEntity, int> _keyAssigner;
        private readonly Dictionary<object, TEntity> _entities;


        public InMemoryEntityCollection()
        {
            _entities = new();
            var type = typeof(TEntity);
            var properties = type.GetProperties();
            var keyAttributes = type.GetProperties()
                .Select(o => new { Property = o, KeyInfo = o.GetCustomAttribute<KeyAttribute>() })
                .Where(o => o.KeyInfo is not null)
                .ToImmutableArray();

            _keyMemberNames = keyAttributes.Select(o => o.Property.Name).ToArray();

            var input = Expression.Variable(typeof(TEntity));

            bool multiKey = keyAttributes.Length > 1;

            if(!multiKey)
            {
                var expression = Expression.Convert(Expression.Property(input, keyAttributes[0].Property), typeof(object));
                _keyProvider = Expression.Lambda<Func<TEntity, object>>(expression, input).Compile();

                if (keyAttributes[0].KeyInfo.AutoIncrement)
                {
                    var propertyType = keyAttributes[0].Property.PropertyType;

                    if (propertyType == typeof(Int32))
                    {
                        var input2 = Expression.Variable(typeof(int));
                        _keyAssigner = Expression.Lambda<Action<TEntity, int>>(Expression.Assign(Expression.Property(input, keyAttributes[0].Property), input2), input, input2).Compile();
                        return;
                    }

                    throw new Exception($"{propertyType} on {type} can not be AutoIncremented");
                }
                return;
            }

            // TODO: Multiple keys...
        }

        public int Count => _entities.Count;

        public bool IsReadOnly => false;

        public void Add(TEntity item)
        {
            _keyAssigner(item, Count);
            _entities.Add(_keyProvider(item), item);
        }

        public void Clear()
        {
            _entities.Clear();
        }

        public bool Contains(TEntity item)
        {
            return _entities.ContainsKey(_keyProvider(item));
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            bool isBinaryExpression = predicate.Body is BinaryExpression;
            (MemberInfo memberInfo, Expression expression)[]? memberExpressions = null;

            if (isBinaryExpression)
            {
                memberExpressions = getMemberName((BinaryExpression)predicate.Body).ToArray();
            }

            bool useKey = memberExpressions.All(o => _keyMemberNames.Contains(o.memberInfo.Name));

            if (useKey)
            {
                var first = memberExpressions.First().expression;

                if (first is ConstantExpression)
                {
                    var keyValue = (first as ConstantExpression).Value;
                    var entity = _entities[keyValue];
                    yield return entity;
                }
                else if(first is MemberExpression)
                {
                    var memberAccess = first as MemberExpression;
                    var captureAccessor = Expression.Lambda<Func<object>>(memberAccess);
                    var entity = _entities[captureAccessor.Compile()];

                    yield return entity;
                }
            }
            else
            {
                var compiledPredicate = predicate.Compile();
                foreach(var entity in _entities.Values)
                {
                    if(compiledPredicate(entity))
                        yield return entity;
                }
            } 
        }

        private IEnumerable<(MemberInfo, Expression)> getMemberName(BinaryExpression binaryExpression)
        {
            if(binaryExpression.Left is MemberExpression)
            {
                if (binaryExpression.Right is ConstantExpression)
                {
                    yield return (((MemberExpression)binaryExpression.Left).Member as MemberInfo, binaryExpression.Right);
                }

                var value = Expression.Lambda(binaryExpression.Right).Compile().DynamicInvoke();
                yield return (((MemberExpression)binaryExpression.Left).Member as MemberInfo, Expression.Constant(value, typeof(object)));

                yield break;
            }
            if(binaryExpression.Right is MemberExpression)
            {
                if (binaryExpression.Left is ConstantExpression)
                {
                    yield return (((MemberExpression)binaryExpression.Right).Member as MemberInfo, binaryExpression.Left);
                }

                var value = Expression.Lambda(binaryExpression.Left).Compile().DynamicInvoke();
                yield return (((MemberExpression)binaryExpression.Right).Member as MemberInfo, Expression.Constant(value, typeof(object)));

                yield break;
            }
        }

        public bool FindFirst(Expression<Func<TEntity, bool>> predicate, out TEntity? entity)
        {
            entity = Find(predicate).FirstOrDefault();
            return entity is not null;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _entities.Values.GetEnumerator();
        }

        public bool Remove(TEntity item)
        {
            return _entities.Remove(_keyProvider(item));
        }

        TEntity IEntityCollection<TEntity>.Find(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }
    }
}
