namespace Miki.Framework
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.EntityFrameworkCore.Storage;
    using Patterns.Repositories;

    public class EntityRepository<T> : IAsyncRepository<T>
        where T : class
    {
        private readonly DbContext context;
        
        public EntityRepository(DbContext context)
        {
            this.context = context;
            this.context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async ValueTask<T> AddAsync(T entity)
        {
            return (await context.Set<T>().AddAsync(entity)).Entity;
        }

        public ValueTask DeleteAsync(T entity)
        {
            context.Set<T>().Remove(entity);
            return default;
        }

        public ValueTask EditAsync(T entity)
        {
            context.Set<T>().Update(entity);
            return default;
        }

        public ValueTask<T> GetAsync(params object[] id)
        {
            return context.Set<T>().FindAsync(id);
            
        }

        public ValueTask<IEnumerable<T>> ListAsync()
        {
            return new ValueTask<IEnumerable<T>>(context.Set<T>());
        }
    }

    public class EntityRepositoryFactory<T> : IRepositoryFactory<T>
        where T : class
    {
        public IAsyncRepository<T> Build(DbContext context)
        {
            return new EntityRepository<T>(context);
        }
    }
}
