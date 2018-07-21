using Miki.Common;
using Miki.Discord.Common;
using Miki.Framework.Events.Commands;
using Miki.Framework.Events.Filters;
using Miki.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public class SimpleCommandHandler : CommandHandler
	{
		public IReadOnlyList<CommandEvent> Commands => map.Commands;
		public IReadOnlyList<Module> Modules => map.Modules;

		public SimpleCommandHandler(CommandMap map)
		{
			this.map = map;
		}

		public override async Task CheckAsync(MessageContext context)
		{
			EventContext e = new EventContext();
			e.commandHandler = this;
			e.message = context.message;
			e.EventSystem = context.eventSystem;

			Stopwatch sw = Stopwatch.StartNew();

			e.Channel = await context.message.GetChannelAsync();

			if (e.Channel is IDiscordGuildChannel guildChannel)
			{
				e.Guild = await guildChannel.GetGuildAsync();
			}

			foreach (PrefixInstance prefix in Prefixes.Values)
			{
				string identifier = prefix.DefaultValue;

				if (e.Guild != null)
				{
					identifier = await prefix.GetForGuildAsync(Bot.Instance.CachePool.Get, e.Guild.Id);
				}

				if (!context.message.Content.StartsWith(identifier))
				{
					continue;
				}

				e.Prefix = prefix;

				string command = Regex.Replace(context.message.Content, @"\r\n?|\n", "")
					.Substring(identifier.Length)
					.Split(' ')
					.First();

				CommandEvent eventInstance = map.GetCommandEvent(command);

				if (eventInstance == null)
				{
					return;
				}

				if ((await GetUserAccessibility(context.message, e.Channel)) >= eventInstance.Accessibility)
				{
					if (await eventInstance.IsEnabled(Bot.Instance.CachePool.Get, (await context.message.GetChannelAsync()).Id))
					{
						await eventInstance.Check(e, identifier);
						await OnMessageProcessed(eventInstance, context.message, sw.ElapsedMilliseconds);
					}
				}
			}
		}

		// TODO: rework this
        public async Task<EventAccessibility> GetUserAccessibility(IDiscordMessage e, IDiscordChannel channel)
        {
			if (e.Author.Id == 121919449996460033)
			{
				return EventAccessibility.DEVELOPERONLY;
			}

			if (channel is IDiscordGuildChannel guildChannel)
			{
				IDiscordGuildUser u = await (await guildChannel.GetGuildAsync()).GetUserAsync(e.Author.Id);
				if (u != null)
				{
					if ((await guildChannel.GetPermissionsAsync(u)).HasFlag(GuildPermission.ManageRoles))
					{
						return EventAccessibility.ADMINONLY;
					}
				}
			}

			return EventAccessibility.PUBLIC;
        }
    }
}