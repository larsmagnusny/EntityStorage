using System.Linq.Expressions;

namespace EntityStorage.Abstractions
{
    public interface IEntityCollection<TEntity> : ICollection<TEntity>
    {
        public bool FindFirst(Expression<Func<TEntity, bool>> predicate, out TEntity? entity);
        public TEntity Find(Expression<Func<TEntity, bool>> predicate);
    }
}
