using Miki.Common;
using Miki.Discord.Common;
using Miki.Framework.Events.Commands;
using Miki.Framework.Events.Filters;
using Miki.Framework.Languages;
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

		public override async Task CheckAsync(EventContext context)
		{
			context.commandHandler = this;
			context.Channel = await context.message.GetChannelAsync();

			if (context.Channel is IDiscordGuildChannel guildChannel)
			{
				context.Guild = await guildChannel.GetGuildAsync();
			}

			foreach (PrefixInstance prefix in Prefixes.Values)
			{
				string identifier = prefix.DefaultValue;

				if (context.Guild != null)
				{
					identifier = await prefix.GetForGuildAsync(Bot.Instance.CachePool.Get, context.Guild.Id);
				}

				if (!context.message.Content.StartsWith(identifier))
				{
					continue;
				}

				context.Prefix = prefix;
				context.Locale = await Locale.GetLanguageInstanceAsync(context.Channel.Id);

                string command = Regex.Replace(context.message.Content, @"\r\n?|\n", "")
                    .Substring(identifier.Length)
                    .Split(' ')
                    .First()
                    .ToLower();

				CommandEvent eventInstance = map.GetCommandEvent(command);

				if (eventInstance == null)
				{
					return;
				}

				if ((await GetUserAccessibility(context)) >= eventInstance.Accessibility)
				{
					if (await eventInstance.IsEnabled(Bot.Instance.CachePool.Get, (await context.message.GetChannelAsync()).Id))
					{
						await eventInstance.Check(context, identifier);
						await OnMessageProcessed(eventInstance, context.message, (DateTime.UtcNow - context.message.Timestamp).Milliseconds);
					}
				}
			}
		}

		// TODO: rework this
		public async Task<EventAccessibility> GetUserAccessibility(IDiscordMessage msg, IDiscordGuildChannel channel)
		{
			return await GetUserAccessibility(new EventContext
			{
				Guild = await channel.GetGuildAsync(),
				Channel = channel,
				message = msg,
			});
		}
        public async Task<EventAccessibility> GetUserAccessibility(EventContext e)
        {
			if (e.Author.Id == 121919449996460033)
			{
				return EventAccessibility.DEVELOPERONLY;
			}

			if (e.Channel is IDiscordGuildChannel guildChannel)
			{
				IDiscordGuildUser u = await e.Guild.GetMemberAsync(e.Author.Id);

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