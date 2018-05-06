using Miki.Framework.Models;
using Miki.Framework.Models.Context;
using Miki.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace Miki.Framework.Events
{
    public class Event
    {
        public string Name { get; set; }

        public EventAccessibility Accessibility { get; set; } = EventAccessibility.PUBLIC;

        public bool OverridableByDefaultPrefix { get; set; } = false;
        public bool CanBeDisabled { get; set; } = true;
        public bool DefaultEnabled { get; set; } = true;

        public Module Module { get; set; }

        public int TimesUsed { get; set; } = 0;

        internal EventSystem eventSystem;

        public ConcurrentDictionary<ulong, bool> cache = new ConcurrentDictionary<ulong, bool>();
        protected Dictionary<ulong, EventCooldownObject> lastTimeUsed = new Dictionary<ulong, EventCooldownObject>();

        public Event()
        {
        }
        public Event(Event eventObject)
        {
            Name = eventObject.Name;
            Accessibility = eventObject.Accessibility;
            OverridableByDefaultPrefix = eventObject.OverridableByDefaultPrefix;
            CanBeDisabled = eventObject.CanBeDisabled;
            DefaultEnabled = eventObject.DefaultEnabled;
            Module = eventObject.Module;
            TimesUsed = eventObject.TimesUsed;
        }
        public Event(Action<Event> info)
        {
            info.Invoke(this);
        }

        public async Task SetEnabled(ulong channelId, bool enabled)
        {
            using (var context = new IAContext())
            {
                CommandState state = await context.CommandStates.FindAsync(Name, channelId.ToDbLong());
                if (state == null)
                {
                    state = context.CommandStates.Add(new CommandState() { ChannelId = channelId.ToDbLong(), CommandName = Name, State = DefaultEnabled }).Entity;
                }
                state.State = enabled;

                cache.AddOrUpdate(channelId, enabled, (x, y) =>
                {
                    return enabled;
                });

                await context.SaveChangesAsync();
            }
        }

        public async Task SetEnabledAll(IGuild guildId, bool enabled)
        {
            var channels = await guildId.GetChannelsAsync();
            foreach (IMessageChannel c in channels)
            {
                await SetEnabled(c.Id, enabled);
            }
        }

        public async Task<bool> IsEnabled(ulong id)
        {
            if (Module != null)
            {
                if (!await Module.IsEnabled(id)) return false;
            }

            if (cache.TryGetValue(id, out bool value))
            {
				return value;
            }
            else
            {
                CommandState state = null;

				using (var context = new IAContext())
				{
					long guildId = id.ToDbLong();
					state = await context.CommandStates.FindAsync(Name, guildId);
				}
                return cache.GetOrAdd(id, state?.State ?? DefaultEnabled);
            }
        }

        public Event SetName(string name)
        {
            Name = name;
            return this;
        }

        public Event SetAccessibility(EventAccessibility accessibility)
        {
            Accessibility = accessibility;
            return this;
        }
    }

    public class EventCooldownObject
    {
        DateTime lastTimeUsed;
        DateTime prevLastTimeUsed;

        DateTime canBeUsedWhen;

        int coolDown = 1;

        public EventCooldownObject(int Cooldown)
        {
            lastTimeUsed = DateTime.Now;
            coolDown = Cooldown;
        }

        public void Tick()
        {
            prevLastTimeUsed = lastTimeUsed;
            lastTimeUsed = DateTime.Now;

            double s = Math.Max(0, coolDown - (lastTimeUsed - prevLastTimeUsed).TotalSeconds);

            canBeUsedWhen = DateTime.Now.AddSeconds(s);
        }

        public bool CanBeUsed()
        {
            return canBeUsedWhen <= DateTime.Now;
        }
    }
}