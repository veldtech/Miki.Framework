using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Miki.Framework.Arguments;
using Miki.Logging;

namespace Miki.Framework.Commands.Nodes
{
    public class NodeNestedExecutable : NodeContainer, IExecutable
    {
        private CommandDelegate _runAsync;

        public NodeNestedExecutable(
            CommandMetadata metadata,
            IServiceProvider builder,
            Type t)
            : this(metadata, null, builder, t)
        {
        }
        public NodeNestedExecutable(
            CommandMetadata metadata,
            NodeContainer parent,
            IServiceProvider builder,
            Type t)
            : base(metadata, parent, builder, t)
        {
        }

        public void SetDefaultExecution(CommandDelegate defaultTask)
        {
            _runAsync = defaultTask;
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
            foreach(var v in Attributes
                .OfType<ICommandRequirement>())
            {
                if(!await v.CheckAsync(context))
                {
                    await v.OnCheckFail(context);
                    return;
                }
            }

            if(_runAsync == null)
            {
                Log.Warning("Default executable not found; omitting request.");
                return;
            }

            await _runAsync(context);
        }
    }
}
