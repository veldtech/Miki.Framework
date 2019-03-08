using Miki.Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Attributes
{
    public abstract class CommandRequirementAttribute : Attribute
    {
        public abstract Task<bool> CheckAsync(ICommandContext e);

        public abstract Task OnCheckFail(ICommandContext e);
    }
}
