using Microsoft.EntityFrameworkCore;
using Miki.Cache;
using Miki.Framework.Models;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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

		protected ConcurrentDictionary<ulong, EventCooldownObject> lastTimeUsed = new ConcurrentDictionary<ulong, EventCooldownObject>();

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

		public string GetCacheKey(ulong channelId)
			=> $"event:{Name}:enabled:{channelId}";

		public async Task SetEnabled(DbContext context, ICacheClient client, ulong channelId, bool enabled)
		{
			CommandState state = await context.Set<CommandState>().FindAsync(Name, channelId.ToDbLong());
			if (state == null)
			{
				state = (await context.Set<CommandState>()
					.AddAsync(new CommandState() { ChannelId = channelId.ToDbLong(), Name = Name, State = DefaultEnabled })).Entity;
			}

			state.State = enabled;

			await client.UpsertAsync(GetCacheKey(channelId), enabled);
		}

		public async Task<bool> IsEnabled(ICacheClient client, ulong id)
		{
			if (Module != null)
			{
				if (!await Module.IsEnabled(client, id)) return false;
			}

			if (await client.ExistsAsync(GetCacheKey(id)))
			{
				return await client.GetAsync<bool>(GetCacheKey(id));
			}
			else
			{
				CommandState state = null;

				long guildId = id.ToDbLong();

				using (var context = MikiApplication.Instance.Information.DatabaseContextFactory())
				{
					state = await context.Set<CommandState>().FindAsync(Name, guildId);
				}

				bool currentState = state?.State ?? DefaultEnabled;
				await client.UpsertAsync(GetCacheKey(id), currentState);
				return currentState;
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
		private DateTime lastTimeUsed;
		private DateTime prevLastTimeUsed;

		private DateTime canBeUsedWhen;

		private readonly int coolDown = 1;

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