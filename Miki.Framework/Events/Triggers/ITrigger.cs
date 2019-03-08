using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Triggers
{
    public interface ITrigger<T>
    {
        Task<EventContext> CheckTrigger(EventContext context, T packet);
    }   
}
