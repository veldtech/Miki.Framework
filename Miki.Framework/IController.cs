using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework
{
    public interface IAsyncEventingExecutor<TRequest> : IAsyncExecutor<TRequest>
    {
        Func<TRequest, Task> OnExecuted { get; set; }
    }

    public interface IAsyncExecutor<in TRequest>
    {
        Task ExecuteAsync(TRequest request);
    }

    public interface IAsyncController<in TRequest, TResponse>
    {
        Task<TResponse> ExecuteAsync(TRequest request);
    }
}
