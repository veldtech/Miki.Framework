using Miki.Common.Events;
using Miki.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
    public delegate Task ProcessCommandDoneEvent(IDiscordMessage m, ICommandEvent e, bool success, float time = 0.0f);

    public class CommandDoneEvent : RuntimeCommandEvent
    {
        public ProcessCommandDoneEvent processEvent;

        public CommandDoneEvent() : base()
        {
        }
        public CommandDoneEvent(Action<CommandDoneEvent> e)
        {
            e.Invoke(this);
        }
    }
}