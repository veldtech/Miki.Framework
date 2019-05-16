using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Miki.Logging;

namespace Miki.Framework.Commands.Nodes
{
    public class NodeExecutable : Node, IExecutable
    {
        internal CommandDelegate runAsync;

        public NodeExecutable(CommandMetadata metadata, CommandDelegate task  = null)
            : base(metadata)
        {
            runAsync = task;
            Requirements = new List<ICommandRequirement>();
        }
        public NodeExecutable(CommandMetadata metadata, NodeContainer parent, CommandDelegate task = null)
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
