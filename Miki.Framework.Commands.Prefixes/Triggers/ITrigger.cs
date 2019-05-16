using Miki.Framework.Commands.Pipelines;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Triggers
{
    public interface ITrigger<T>
    {
        ValueTask<string> CheckTriggerAsync(IMutableContext context, T packet);
    }   
}
