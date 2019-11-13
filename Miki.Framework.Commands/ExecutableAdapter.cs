namespace Miki.Framework.Commands
{
    using System;
    using System.Threading.Tasks;

    public class ExecutableAdapter : IExecutable
    {
        private readonly Func<IContext, ValueTask> task;

        public ExecutableAdapter(Func<IContext, ValueTask> taskFactory)
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
