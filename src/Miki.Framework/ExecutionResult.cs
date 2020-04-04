namespace Miki.Framework
{
    using System;
    
    /// <inheritdoc/>
    public class ExecutionResult<T> : IExecutionResult<T>
    {
        /// <inheritdoc/>
        public bool Success 
            => Error == null;

        /// <inheritdoc/>
        public Exception Error { get; }

        /// <inheritdoc/>
        public T Result { get; }

        /// <inheritdoc/>
        public IContext Context { get; }

        /// <inheritdoc/>
        public long TimeMilliseconds { get; }

        /// <summary>
        /// Constructs a new Execution result.
        /// </summary>
        public ExecutionResult(IContext context, T result, long timeMs, Exception error = null)
        {
            Context = context;
            Result = result;
            TimeMilliseconds = timeMs;
            Error = error;
        }
    }
}
