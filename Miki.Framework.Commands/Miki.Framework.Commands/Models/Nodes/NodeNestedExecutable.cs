using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miki.Framework.Arguments;

namespace Miki.Framework.Commands.Nodes
{
    public class NodeNestedExecutable : NodeContainer, IExecutable
    {
        private Func<IContext, Task> runAsync;

        public NodeNestedExecutable(
            CommandMetadata metadata, 
            Func<IContext, Task> defaultTask = null)
            : base(metadata)
        {
            runAsync = defaultTask;
            Requirements = new List<ICommandRequirement>();
        }
        public NodeNestedExecutable(
            CommandMetadata metadata, 
            NodeContainer parent, 
            Func<IContext, Task> defaultTask = null)
            : base(metadata, parent)
        {
            runAsync = defaultTask;
            Requirements = new List<ICommandRequirement>();
        }

        public void SetDefaultExecution(Func<IContext, Task> defaultTask)
        {
            runAsync = defaultTask;
        }

        public override Node FindCommand(IArgumentPack pack)
        {
            var arg = pack.Peek()
                .ToLowerInvariant();

            if (Metadata.Identifiers != null
                && Metadata.Identifiers.Count() > 0)
            {
                if (Metadata.Identifiers.Any(x => x == arg))
                {
                    pack.Take();
                    var cmd = base.FindCommand(pack);
                    if(cmd != null)
                    {
                        return cmd;
                    }
                    return this;
                }
            }
            return null;
        }

        public async Task RunAsync(IContext context)
        {
            foreach(var v in Requirements)
            {
                if(!await v.CheckAsync(context))
                {
                    await v.OnCheckFail(context);
                    return;
                }
            }

            await runAsync(context);
        }
    }
}
