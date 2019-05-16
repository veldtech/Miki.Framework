using Miki.Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public abstract class CommandRequirementAttribute : Attribute, ICommandRequirement
    {
        public abstract ValueTask<bool> CheckAsync(IContext e);
        public abstract Task OnCheckFail(IContext e);
    }
}
