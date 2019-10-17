namespace Miki.Framework
{
    using System;
    using System.Threading.Tasks;
    using Patterns.Repositories;

    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Gets a repository with the supported type
        /// </summary>
        /// <typeparam name="T">Type of data you need a repository for.</typeparam>
        IAsyncRepository<T> GetRepository<T>() 
            where T : class;

        /// <summary>
        /// Commits and flushes your work to the datasource.
        /// </summary>
        ValueTask CommitAsync();
    }
}
