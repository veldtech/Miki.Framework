using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Miki.Framework.Events;

namespace Miki.Framework.Commands.Nodes
{
    public class NodeNestedExecutable : NodeContainer
    {
        public readonly List<ICommandRequirement> Requirements = new List<ICommandRequirement>();

        private Func<IContext, Task> runAsync;

        public NodeNestedExecutable(
            CommandMetadata metadata, 
            Func<IContext, Task> defaultTask = null)
            : base(metadata)
        {
            runAsync = defaultTask;
        }
        public NodeNestedExecutable(
            CommandMetadata metadata, 
            NodeContainer parent, 
            Func<IContext, Task> defaultTask = null)
            : base(metadata, parent)
        {
            runAsync = defaultTask;
        }

        public override async Task RunAsync(IContext e)
        {
            if (e.GetArgumentPack().CanTake)
            {
                var command = e.GetArgumentPack().Pack
                    .Peek().ToLowerInvariant();
                foreach (var c in Children)
                {
                    if (c.Metadata.Name.ToLowerInvariant() == command)
                    {
                        await c.RunAsync(e);
                        return;
                    }
                }
            }

            if (runAsync != null)
            {
                await runAsync(e);
            }
        }

        public void SetDefaultExecution(Func<IContext, Task> defaultTask)
        {
            runAsync = defaultTask;
        }
    }
}
