namespace Miki.Framework
{
    using System;
    using System.Threading.Tasks;
    using Patterns.Repositories;

    public interface IUnitOfWork : IDisposable
    {
        IAsyncRepository<T> GetRepository<T>() 
            where T : class;

        ValueTask CommitAsync();
    }
}
