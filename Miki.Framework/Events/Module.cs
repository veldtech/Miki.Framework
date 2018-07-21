using Miki.Framework.Models;
using Miki.Framework.Models.Context;
using Miki.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Miki.Discord.Common;
using Miki.Cache;

namespace Miki.Framework.Events
{
    public class Module
    {
        public string Name { get; set; }

        public bool Nsfw { get; set; } = false;
        public bool Enabled { get; set; } = true;
        public bool CanBeDisabled { get; set; } = true;

        public Func<IDiscordMessage, Task> MessageRecieved { get; set; }
        public Func<IDiscordUser, IDiscordUser, Task> UserUpdated { get; set; }
        public Func<IDiscordUser, Task> UserJoinGuild { get; set; }
        public Func<IDiscordUser, Task> UserLeaveGuild { get; set; }
		public Func<IDiscordGuild, Task> JoinedGuild { get; set; } 
        public Func<IDiscordGuild, Task> LeftGuild { get; set; }

        public List<CommandEvent> Events { get; set; } = new List<CommandEvent>();
        public List<BaseService> Services { get; set; } = new List<BaseService>();

        internal EventSystem EventSystem;

		public string SqlName => "module:" + Name;

		public object InstanceOf { get; private set; }

		private bool isInstalled = false;

        internal Module(object instanceOf = null)
        {
			InstanceOf = instanceOf;
		}

		public Module AddCommand(CommandEvent command)
		{
			Events.Add(command);
			return this;
		}

		public string GetCacheKey(ulong id)
			=> $"module:{Name}:enabled:{id}";

		public void Install(Bot bot, EventSystem system)
        {
			this.EventSystem = system;
			Name = Name.ToLower();

            foreach (CommandEvent e in Events)
            {
				e.Module = this;
            }

			foreach(BaseService s in Services)
			{
				s.Install(this, bot);
			}

            isInstalled = true;
        }

		public async Task<bool> IsEnabled(ICacheClient cache, ulong id)
		{
			if (await cache.ExistsAsync(GetCacheKey(id)))
			{
				return await cache.GetAsync<bool>(GetCacheKey(id));
			}
			else
			{
				ModuleState state = null;

				using (var context = new IAContext())
				{
					long guildId = id.ToDbLong();
					state = await context.ModuleStates.FindAsync(SqlName, guildId);
				}

				if (state == null)
				{
					await cache.UpsertAsync(GetCacheKey(id), Enabled);
					return Enabled;
				}

				await cache.UpsertAsync(GetCacheKey(id), state.State);
				return state.State;
			}
		}

		public void Uninstall(object bot)
        {
            Bot b = (Bot)bot;

            if (!isInstalled)
            {
                return;
            }

            isInstalled = false;
        }

		public object GetReflectedInstance()
		{
			return InstanceOf;
		}

		internal void SetInstance(object instance)
		{
			InstanceOf = instance;
		}

        public Module SetNsfw(bool val)
        {
            Nsfw = val;
            return this;
        }

        public async Task SetEnabled(ICacheClient cache, ulong channelId, bool enabled)
        {
            using (var context = new IAContext())
            {
                ModuleState state = await context.ModuleStates.FindAsync(SqlName, channelId.ToDbLong());
                if (state == null)
                {
                    state = context.ModuleStates.Add(new ModuleState()
					{
						ChannelId = channelId.ToDbLong(),
						ModuleName = SqlName,
						State = Enabled
					}).Entity;
                }
                state.State = enabled;

                await cache.UpsertAsync(GetCacheKey(channelId), enabled);
                await context.SaveChangesAsync();
            }
        }

    }
}