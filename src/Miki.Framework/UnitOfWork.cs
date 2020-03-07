namespace Miki.Framework
{
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Patterns.Repositories;

    /// <summary>
    /// Default unit of work pattern, uses Entity Framework by default.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext context;

        /// <summary>
        /// Default unit of work pattern, uses Entity Framework by default.
        /// </summary>
        public UnitOfWork(DbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public IAsyncRepository<T> GetRepository<T>([Optional] IRepositoryFactory<T> factory)
            where T : class
        {
            if (factory == null)
            {
                return new EntityRepository<T>(context);
            }

            return factory.Build(context);
        }

        /// <inheritdoc />
        public ValueTask CommitAsync()
        {
            return new ValueTask(context.SaveChangesAsync());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            context.Dispose();
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return context.DisposeAsync();
        }
    }
}
