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

    /// <summary>
    /// Default controller pipeline object. Expects a valid request and and will give back a valid
    /// response if no exceptions are thrown. Used to abstractify entities.
    /// </summary>
    /// <typeparam name="TRequest">Required request object that the controller requires to handle its
    /// request properly.</typeparam>
    /// <typeparam name="TResponse">Response object that handles </typeparam>
    public interface IAsyncController<in TRequest, TResponse>
    {
        ValueTask<TResponse> ExecuteAsync(TRequest request);
    }
}
