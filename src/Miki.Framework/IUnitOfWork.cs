namespace Miki.Framework
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Patterns.Repositories;

    /// <summary>
    /// A pattern to abstractify transactions from storage APIs.
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Gets the repository for a type. Will by default create an <see cref="EntityRepository{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        IAsyncRepository<T> GetRepository<T>([Optional] IRepositoryFactory<T> factory)
            where T : class;

        /// <summary>
        /// Commits and flushes your work to the datasource.
        /// </summary>
        ValueTask CommitAsync();
    }
}
