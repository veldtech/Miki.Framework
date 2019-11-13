namespace Miki.Framework
{
    using Microsoft.EntityFrameworkCore;
    using Patterns.Repositories;

    public interface IRepositoryFactory<T>
        where T : class
    {
        IAsyncRepository<T> Build(DbContext context);
    }
}
