namespace Miki.Framework
{
    using System;
    
    public class ExecutionResult<T> : IExecutionResult<T>
    {
        public bool Success 
            => Error == null;

        public Exception Error { get; }
        
        public T Result { get; }

        public IContext Context { get; }

        public ExecutionResult(IContext context, T result, Exception error = null)
        {
            Context = context;
            Result = result;
            Error = error;
        }
    }
}
