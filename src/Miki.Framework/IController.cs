namespace Miki.Framework
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Result of an execution.
    /// </summary>
    public interface IExecutionResult<out T>
    {
        /// <summary>
        /// Returns whether the execution was successful or not.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// If the execution was not successful, there will be an error in this object that you can use
        /// to log.
        /// </summary>
        Exception Error { get; }

        /// <summary>
        /// If the execution was successful, there will be a result payload in here.
        /// </summary>
        T Result { get; }

        /// <summary>
        /// The context used to execute this request.
        /// </summary>
        IContext Context { get; }

        /// <summary>
        /// The amount of milliseconds this request ran for.
        /// </summary>
        long TimeMilliseconds { get; }
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
