using Miki.Discord;
using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public abstract class CommandRequirementAttribute : Attribute
    {
        public abstract Task<bool> CheckAsync(IContext e);
        public abstract Task OnCheckFail(IContext e);
    }
}
