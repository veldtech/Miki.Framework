using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework
{
    public interface IExecutionResult<out T>
    {
        bool Success { get; }

        Exception Error { get; }

        T Result { get; }

        IContext Context { get; }
    }

    public interface IAsyncEventingExecutor<TRequest> : IAsyncExecutor<TRequest>
    {
        Func<IExecutionResult<TRequest>, ValueTask> OnExecuted { get; set; }
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
