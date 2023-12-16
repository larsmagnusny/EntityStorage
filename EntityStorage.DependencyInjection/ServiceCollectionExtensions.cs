using EntityStorage.Abstractions;
using EntityStorage.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace EntityStorage.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryEntityCollection<TEntity>(this IServiceCollection services)
        {
            return services.AddTransient<IEntityCollection<TEntity>, EntityCollection<TEntity>>();
        }

        public static IServiceCollection AddEntityCollection<TEntity>(this IServiceCollection services)
        {
            return services.AddTransient<IEntityCollection<TEntity>, EntityCollection<TEntity>>();
        }
    }
}