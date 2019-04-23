using Miki.Framework.Commands.Models;
using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public class CommandNode 
    {
        public IExecutableCommand InnerCommand { get; }
        public CommandMetadata Metadata { get; }
        public CommandNode Parent { get; }
        public List<CommandNode> Children { get; }

        public async Task RunAsync(CommandContext e)
        {   
            if(InnerCommand == null)
            {
                return;
            }
            
            await InnerCommand.ExecuteAsync(e);
        }

        public override string ToString()
        {
            if(Parent == null
                || string.IsNullOrEmpty(Parent.ToString()))
            {
                return Metadata.Name.ToLowerInvariant();
            }
            return $"{Parent.ToString()}.{Metadata.Name.ToLowerInvariant()}";
        }

        private string GetCacheKey(ulong channelId)
            => $"ev:state:{channelId}";
    }
}
