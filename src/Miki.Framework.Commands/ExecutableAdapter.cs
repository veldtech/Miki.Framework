using System;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExecutableAdapter'
    public class ExecutableAdapter : IExecutable
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExecutableAdapter'
    {
        private readonly Func<IContext, ValueTask> task;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExecutableAdapter.ExecutableAdapter(Func<IContext, ValueTask>)'
        public ExecutableAdapter(Func<IContext, ValueTask> taskFactory)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExecutableAdapter.ExecutableAdapter(Func<IContext, ValueTask>)'
        {
            task = taskFactory;
        }

        /// <inheritdoc />
        public ValueTask ExecuteAsync(IContext request)
        {
            return task(request);
        }
    }
}
