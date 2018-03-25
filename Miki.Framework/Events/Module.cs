using Miki.Framework.Models;
using Miki.Framework.Models.Context;
using Miki.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Miki.Framework.Events
{
    public class Module
    {
        public string Name { get; set; } = "";
        public bool Nsfw { get; set; } = false;
        public bool Enabled { get; set; } = true;
        public bool CanBeDisabled { get; set; } = true;

        public Mutex threadLock;

        public Func<SocketMessage, Task> MessageRecieved { get; set; } = null;
        public Func<IUser, IUser, Task> UserUpdated { get; set; } = null;
        public Func<IUser, Task> UserJoinGuild { get; set; } = null;
        public Func<IUser, Task> UserLeaveGuild { get; set; } = null;
		public Func<IGuild, Task> JoinedGuild { get; set; } = null;
        public Func<IGuild, Task> LeftGuild { get; set; } = null;

        public List<CommandEvent> Events { get; set; } = new List<CommandEvent>();
        public List<BaseService> Services { get; set; } = new List<BaseService>();

        private ConcurrentDictionary<ulong, bool> cache = new ConcurrentDictionary<ulong, bool>();

        internal EventSystem EventSystem;

        public string SqlName
        {
            get
            {
                return "module:" + Name;
            }
        }

        private bool isInstalled = false;

        internal Module()
        {
        }
        public Module(string name, bool enabled = true)
        {
            Name = name;
            Enabled = enabled;
        }
        public Module(Action<Module> info)
        {
            info.Invoke(this);
        }

        public void Install(object bot)
        {
            Bot b = (Bot)bot;
            Name = Name.ToLower();

            if (MessageRecieved != null)
            {
                b.Client.MessageReceived += Module_MessageReceived;
            }

            if (UserUpdated != null)
            {
                b.Client.UserUpdated += Module_UserUpdated;
            }

            if (UserJoinGuild != null)
            {
                b.Client.UserJoined += Module_UserJoined;
            }

            if (UserLeaveGuild != null)
            {
                b.Client.UserLeft += Module_UserLeft;
            }

            if (JoinedGuild != null)
            {
                b.Client.JoinedGuild += Module_JoinedGuild;
            }

            if (LeftGuild != null)
            {
                b.Client.LeftGuild += Module_LeftGuild;
            }

            EventSystem.CommandHandler.AddModule(this);

            foreach (CommandEvent e in Events)
            {
				e.eventSystem = EventSystem.Instance;
				e.Module = this;

				EventSystem.CommandHandler.AddCommand(e);
            }

            isInstalled = true;
        }

        public Module AddCommand(CommandEvent command)
        {
            Events.Add(command);
            return this;
        }

        public void Uninstall(object bot)
        {
            Bot b = (Bot)bot;

            if (!isInstalled)
            {
                return;
            }

            EventSystem.Modules.Remove(Name);
            EventSystem.CommandHandler.AddModule(this);

             if (MessageRecieved != null)
             {
                 b.Client.MessageReceived -= Module_MessageReceived;
             }

             if (UserUpdated != null)
             {
                 b.Client.UserUpdated -= Module_UserUpdated;
             }

             if (UserJoinGuild != null)
             {
                 b.Client.UserJoined -= Module_UserJoined;
             }

             if (UserLeaveGuild != null)
			 {
                 b.Client.UserLeft -= Module_UserLeft;
             }

             if (JoinedGuild != null)
             {
				b.Client.JoinedGuild -= Module_JoinedGuild;
             }

             if (LeftGuild != null)
             {
                 b.Client.LeftGuild -= Module_LeftGuild;
             }

            isInstalled = false;
        }

        private async Task Module_JoinedGuild(IGuild arg)
        {
            if (await IsEnabled(arg.Id))
            {
				try
				{
					await JoinedGuild(arg);
				}
				catch(Exception e)
				{
					Log.Error(e.ToString());
				}
            }
        }

        public Module SetNsfw(bool val)
        {
            Nsfw = val;
            return this;
        }

        private async Task Module_LeftGuild(IGuild arg)
        {
			if (await IsEnabled(arg.Id))
			{
				try
				{
					await LeftGuild(arg);
				}
				catch (Exception e)
				{
					Log.Error(e.ToString());
				}
			}
		}

        private async Task Module_UserJoined(IUser arg)
        {
			if (arg is IGuildUser guildUser)
			{
				if (await IsEnabled(guildUser.Guild.Id))
				{
					try
					{
						await UserJoinGuild(arg);
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}
        }

        private async Task Module_UserLeft(IUser arg)
        {
			if (arg is IGuildUser guildUser)
			{
				if (await IsEnabled(guildUser.Guild.Id))
				{
					try
					{
						await UserLeaveGuild(arg);
					}
					catch (Exception e)
					{
						Log.Error(e.ToString());
					}
				}
			}
        }

        private async Task Module_UserUpdated(IUser arg1, IUser arg2)
        {
			if (arg1 is IGuildUser guildUser)
			{
				if (await IsEnabled(guildUser.GuildId))
                {
					try {
						await UserUpdated(guildUser, arg2);
					}
					catch (Exception e)
					{
						Log.Error(e.ToString());
					}
				}
            }
        }

        private async Task Module_MessageReceived(SocketMessage message)
        {
			IGuildChannel guildChannel = (message.Channel as IGuildChannel);

			if (guildChannel == null)
				return;

            if (await IsEnabled(guildChannel.Guild.Id))
            {
				await MessageRecieved(message);
            }
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
    }
}