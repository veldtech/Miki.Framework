namespace Miki.Bot.Models.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Miki.Patterns.Repositories;

    public class EntityRepository<T> : IAsyncRepository<T>
        where T : class
    {
        private readonly DbContext context;

        public EntityRepository(DbContext context)
        {
            this.context = context;
        }

        public ValueTask AddAsync(T entity)
        {
            context.Set<T>().Add(entity);
            return default;
        }

        public ValueTask DeleteAsync(T entity)
        {
            context.Set<T>().Remove(entity);
            return default;
        }

        public ValueTask EditAsync(T entity)
        {

            context.Entry(entity).DetectChanges();
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
}
