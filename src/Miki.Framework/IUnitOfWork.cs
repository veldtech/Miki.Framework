namespace Miki.Framework
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Patterns.Repositories;

    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// 
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
