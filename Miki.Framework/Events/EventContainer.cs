using System.Collections.Generic;
using System.Linq;

namespace Miki.Framework.Events
{
    internal class EventContainer
    {
        public Dictionary<string, CommandEvent> CommandEvents { private set; get; } = new Dictionary<string, CommandEvent>();
        public Dictionary<string, CommandDoneEvent> CommandDoneEvents { private set; get; } = new Dictionary<string, CommandDoneEvent>();

        /// <summary>
        /// I use this to store internal events.
        /// </summary>
        internal Dictionary<string, Event> InternalEvents { private set; get; } = new Dictionary<string, Event>();

        public Event GetEvent(string name)
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

        public Event[] GetAllEvents()
        {
            List<Event> allEvents = new List<Event>();
            allEvents.AddRange(CommandEvents.Values);
            return allEvents.ToArray();
        }

        public Dictionary<string, Event> GetAllEventsDictionary()
        {
            Dictionary<string, Event> allEvents = new Dictionary<string, Event>();
            CommandEvents.ToList().ForEach(x => allEvents.Add(x.Key, x.Value));
            return allEvents;
        }
    }
}