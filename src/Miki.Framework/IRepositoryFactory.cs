namespace Miki.Framework
{
    using Microsoft.EntityFrameworkCore;
    using Patterns.Repositories;

    /// <summary>
    /// Can be used to inject custom logic per type.
    /// </summary>
    public interface IRepositoryFactory<T>
        where T : class
    {
        /// <summary>
        /// Builds a repository to perform CRUD operations with.
        /// </summary>
        IAsyncRepository<T> Build(DbContext context);
    }
}
