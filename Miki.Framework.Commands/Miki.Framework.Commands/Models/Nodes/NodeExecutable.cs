using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Miki.Framework.Events;
using Miki.Logging;

namespace Miki.Framework.Commands.Nodes
{
    public class NodeExecutable : Node, IExecutable
    {
        internal Func<IContext, Task> runAsync;

        public NodeExecutable(CommandMetadata metadata, Func<IContext, Task> task  = null)
            : base(metadata)
        {
            runAsync = task;
            Requirements = new List<ICommandRequirement>();
        }
        public NodeExecutable(CommandMetadata metadata, NodeContainer parent, Func<IContext, Task> task = null)
            : base(metadata, parent)
        {
            runAsync = task;
            Requirements = new List<ICommandRequirement>();
        }

        public async Task RunAsync(IContext e)
        {
            if(runAsync == null)
            {
                throw new InvalidProgramException("Invalid method bindings for command " + ToString());
            }

            foreach(var req in Requirements)
            {
                if(!await req.CheckAsync(e))
                {
                    await req.OnCheckFail(e);
                    return;
                }
            }
            await runAsync(e);
        }
    }
}
