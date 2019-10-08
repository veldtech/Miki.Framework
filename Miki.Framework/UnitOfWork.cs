namespace Miki.Bot.Models.Repositories
{
    using System.Threading.Tasks;
    using Framework;
    using Microsoft.EntityFrameworkCore;
    using Patterns.Repositories;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext context;

        public UnitOfWork(DbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public IAsyncRepository<T> GetRepository<T>()
            where T : class
        {
            return new EntityRepository<T>(context);
        }

        /// <inheritdoc />
        public ValueTask CommitAsync()
        {
            return new ValueTask(context.SaveChangesAsync());
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
