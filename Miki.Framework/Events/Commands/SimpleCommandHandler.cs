using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miki.Cache;
using Miki.Discord.Common;
using Miki.Framework.Events.Commands;
using Miki.Framework.Languages;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public class SimpleCommandHandler : CommandHandler
	{
		public IReadOnlyList<CommandEvent> Commands => _map.Commands;
		public IReadOnlyList<Module> Modules => _map.Modules;

		public SimpleCommandHandler(ICacheClient pool, CommandMap map)
			: base(pool)
		{
			_map = map;
		}

		public override async Task CheckAsync(EventContext context)
		{
			var stopWatch = Stopwatch.StartNew();

			context.commandHandler = this;
			context.Channel = await context.message.GetChannelAsync();

			if (context.Channel is IDiscordGuildChannel guildChannel)
			{
				context.Guild = await guildChannel.GetGuildAsync();
			}

			foreach (PrefixInstance prefix in Prefixes)
			{
				string identifier = prefix.DefaultValue;

				if (context.Guild != null)
				{
					identifier = await prefix.GetForGuildAsync(MikiApp.Instance.Discord.CacheClient, context.Guild.Id);
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

				if (_map.TryGetCommandEvent(command, out var eventInstance))
				{
					if (eventInstance == null)
					{
						return;
					}

					if (eventInstance.Accessibility != EventAccessibility.PUBLIC)
					{
						if ((await GetUserAccessibility(context)) < eventInstance.Accessibility)
						{
							return;
						}
					}

					if (await eventInstance.IsEnabled(
                        context.Services.GetService<ICacheClient>(), 
                        context.Services.GetService<DbContext>(), 
                        (await context.message.GetChannelAsync()).Id))
					{
						await eventInstance.Check(context, identifier);
						await OnMessageProcessed(eventInstance, context.message, stopWatch.ElapsedMilliseconds);
					}
				}
			}
		}

		// TODO: rework this
		public async Task<EventAccessibility> GetUserAccessibility(IDiscordMessage msg, IDiscordChannel channel)
		{
			if (channel is IDiscordGuildChannel c)
			{
				return await GetUserAccessibility(new EventContext
				{
					Guild = await c.GetGuildAsync(),
					Channel = channel as IDiscordTextChannel,
					message = msg,
				});
			}
			return EventAccessibility.ADMINONLY;
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

		public CommandEvent GetCommandById(string id)
			=> _map.GetCommandEvent(id);
	}
}