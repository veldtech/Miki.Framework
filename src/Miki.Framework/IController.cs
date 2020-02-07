namespace Miki.Framework
{
    using System;
    using System.Threading.Tasks;

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
        ValueTask ExecuteAsync(TRequest request);
    }

    public interface IAsyncController<in TRequest, TResponse>
    {
        ValueTask<TResponse> ExecuteAsync(TRequest request);
    }
}
