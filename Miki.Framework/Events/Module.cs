using Miki.Framework.Models;
using Miki.Framework.Models.Context;
using Miki.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Miki.Discord.Common;

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

        private ConcurrentDictionary<ulong, bool> cache = new ConcurrentDictionary<ulong, bool>();

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

		public async Task<bool> IsEnabled(ulong id)
		{
			ModuleState state = null;

			if (cache.ContainsKey(id))
			{
				return cache.GetOrAdd(id, Enabled);
			}
			else
			{
				using (var context = new IAContext())
				{
					long guildId = id.ToDbLong();
					state = await context.ModuleStates.FindAsync(SqlName, guildId);
				}

				if (state == null)
				{
					return cache.GetOrAdd(id, Enabled);
				}

				return cache.GetOrAdd(id, state.State);
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

        public async Task SetEnabled(ulong serverId, bool enabled)
        {
            using (var context = new IAContext())
            {
                ModuleState state = await context.ModuleStates.FindAsync(SqlName, serverId.ToDbLong());
                if (state == null)
                {
                    state = context.ModuleStates.Add(new ModuleState() { ChannelId = serverId.ToDbLong(), ModuleName = SqlName, State = Enabled }).Entity;
                }
                state.State = enabled;

                cache.AddOrUpdate(serverId, enabled, (x, y) =>
                {
                    return enabled;
                });

                await context.SaveChangesAsync();
            }
        }

    }
}