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
        public Func<IDiscordGuildUser, Task> UserJoinGuild { get; set; }
        public Func<IDiscordGuildUser, Task> UserLeaveGuild { get; set; }
		public Func<IDiscordGuild, Task> JoinedGuild { get; set; } 
        public Func<IDiscordGuild, Task> LeftGuild { get; set; }

        public List<CommandEvent> Events { get; set; } = new List<CommandEvent>();
        public List<BaseService> Services { get; set; } = new List<BaseService>();

        public EventSystem EventSystem;

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

		public void Install(EventSystem system)
        {
			EventSystem = system;
			Name = Name.ToLower();

			if(MessageRecieved != null)
			{
				system.bot.Client.MessageCreate += async (message) =>
				{
					await HandleEvent(MessageRecieved(message), message.ChannelId);
				};
			}

			if (UserJoinGuild != null)
			{
				system.bot.Client.GuildMemberCreate += async (guildMember) =>
				{
					await UserJoinGuild(guildMember);
				};
			}

			if (UserLeaveGuild != null)
			{
				system.bot.Client.GuildMemberDelete += async (guildMember) =>
				{
					await UserLeaveGuild(guildMember);
				};
			}

			if (UserUpdated != null)
			{
				system.bot.Client.UserUpdate += async (oldUser, newUser) =>
				{
					await UserUpdated(oldUser, newUser);
				};
			}

			foreach (CommandEvent e in Events)
            {
				e.Module = this;
            }

			foreach(BaseService s in Services)
			{
				s.Install(this);
			}

            isInstalled = true;
        }
		
		private async Task HandleEvent(Task runnableEvent, ulong channelId)
		{
			if (await IsEnabled(await EventSystem.bot.CachePool.GetAsync(), channelId))
			{
				await runnableEvent;
			}
		}

		public async Task<bool> IsEnabled(ICacheClient cache, ulong channelId)
		{
			if (await cache.ExistsAsync(GetCacheKey(channelId)))
			{
				return await cache.GetAsync<bool>(GetCacheKey(channelId));
			}
			else
			{
				ModuleState state = null;

				using (var context = new IAContext())
				{
					long id = channelId.ToDbLong();
					state = await context.ModuleStates.FindAsync(SqlName, id);
				}

				if (state == null)
				{
					await cache.UpsertAsync(GetCacheKey(channelId), Enabled);
					return Enabled;
				}

				await cache.UpsertAsync(GetCacheKey(channelId), state.State);
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