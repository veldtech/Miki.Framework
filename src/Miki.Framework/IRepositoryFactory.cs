using Microsoft.EntityFrameworkCore;
using Miki.Patterns.Repositories;

namespace Miki.Framework
{
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
