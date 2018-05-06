using Discord;
using Miki.Common;
using Miki.Framework.Events.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
    public class CommandHandlerBuilder
    {
		public MessageFilter messageFilter = new MessageFilter();

        private CommandHandler commandHandler = null;

        public CommandHandlerBuilder(EventSystem eventSystem)
        {
            commandHandler = new CommandHandler(eventSystem, new CommandMap());
        }

        public CommandHandlerBuilder AddCommand(CommandEvent cmd)
        {
			(cmd as CommandEvent).eventSystem = commandHandler.eventSystem;
            commandHandler.AddCommand(cmd);
            return this;
        }

        public CommandHandlerBuilder AddModule(Module module)
        {
            commandHandler.AddModule(module);
            return this;
        }

        public CommandHandlerBuilder AddPrefix(string value)
        {
            if (!commandHandler.Prefixes.ContainsKey(value))
            {
                commandHandler.Prefixes.Add(value, new PrefixInstance(value, false, false));
            }
            return this;
        }

        public CommandHandler Build()
        {
            return commandHandler;
        }
    }

	public class CommandHandler
	{
		public IReadOnlyList<CommandEvent> Commands => map.Commands;
		public IReadOnlyList<Module> Modules => map.Modules;

		public async Task<string> GetPrefixAsync(ulong guildId)
			=> await Prefixes.FirstOrDefault(x => x.Value.IsDefault).Value.GetForGuildAsync(guildId);

		public DateTime TimeCreated = DateTime.Now;
		internal DateTime timeDisposed;

		internal EventSystem eventSystem;

		public Dictionary<string, PrefixInstance> Prefixes { get; private set; } = new Dictionary<string, PrefixInstance>();

		private CommandMap map;

		public CommandHandler(EventSystem eventSystem, CommandMap map)
		{
			this.eventSystem = eventSystem;
			this.map = map;
		}

		public bool ShouldDispose()
		{
			return (DateTime.Now > timeDisposed);
		}

		public async Task CheckAsync(IMessage msg)
		{
			foreach (PrefixInstance prefix in Prefixes.Values)
			{
				string identifier = prefix.DefaultValue;

				if (msg.Channel is IGuildChannel channel)
				{
					identifier = await prefix.GetForGuildAsync(channel.GuildId);
				}

				if (!msg.Content.StartsWith(identifier))
				{
					continue;
				}

				string command = Regex.Replace(msg.Content, @"\r\n?|\n", "")
					.Substring(identifier.Length)
					.Split(' ')
					.First();

				CommandEvent eventInstance = map.GetCommandEvent(command);

				if (eventInstance == null)
				{
					return;
				}

				if (GetUserAccessibility(msg) >= eventInstance.Accessibility)
				{
					if (await eventInstance.IsEnabled(msg.Channel.Id))
					{
						await eventInstance.Check(msg, this, identifier);
					}
				}
			}
		}

        public void AddCommand(CommandEvent cmd)
        {
			map.AddCommand(cmd);
        }

        public void AddModule(Module module)
        {
            map.AddModule(module);
        }

		// TODO: rework this
        public EventAccessibility GetUserAccessibility(IMessage e)
        {
            if (e.Channel == null)
				return EventAccessibility.PUBLIC;

			//if (eventSystem.DeveloperIds.Contains(e.Author.Id))
				//return EventAccessibility.DEVELOPERONLY;

			if ((e.Author as IGuildUser).GuildPermissions.Has(GuildPermission.ManageRoles))
				return EventAccessibility.ADMINONLY;

			return EventAccessibility.PUBLIC;
        }
    }

	public interface ICommandHandler
	{
		Task CheckAsync(IMessage message);
	}
	
	public struct CommandSession
	{
		public ulong UserId;
		public ulong ChannelId;

		public CommandSession(ulong user, ulong channel)
		{
			UserId = user;
			ChannelId = channel;
		}
	}
}