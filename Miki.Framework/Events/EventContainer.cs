using Miki.Common.Events;
using System.Collections.Generic;
using System.Linq;

namespace Miki.Framework.Events
{
    internal class EventContainer
    {
        public Dictionary<string, ICommandEvent> CommandEvents { private set; get; } = new Dictionary<string, ICommandEvent>();
        public Dictionary<string, CommandDoneEvent> CommandDoneEvents { private set; get; } = new Dictionary<string, CommandDoneEvent>();

        /// <summary>
        /// I use this to store internal events.
        /// </summary>
        internal Dictionary<string, Event> InternalEvents { private set; get; } = new Dictionary<string, Event>();

        public IEvent GetEvent(string name)
        {
            if (CommandEvents.ContainsKey(name))
            {
                return CommandEvents[name];
            }
            return null;
        }

        public Event GetInternalEvent(string name)
        {
            return InternalEvents[name];
        }

        public IEvent[] GetAllEvents()
        {
            List<IEvent> allEvents = new List<IEvent>();
            allEvents.AddRange(CommandEvents.Values);
            return allEvents.ToArray();
        }

        public Dictionary<string, IEvent> GetAllEventsDictionary()
        {
            Dictionary<string, IEvent> allEvents = new Dictionary<string, IEvent>();
            CommandEvents.ToList().ForEach(x => allEvents.Add(x.Key, x.Value));
            return allEvents;
        }
    }
}