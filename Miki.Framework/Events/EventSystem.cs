using Miki.Framework.Events.Attributes;
using Miki.Framework.Models;
using Miki.Framework.Models.Context;
using Miki.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Miki.Framework.Events.Filters;
using Miki.Framework.Events.Commands;
using Miki.Framework.Exceptions;
using Miki.Framework.Extension;
using Miki.Framework.Languages;
using Miki.Discord.Rest.Entities;
using Miki.Discord.Common;
using Miki.Discord;
using Miki.Discord.Internal;

namespace Miki.Framework.Events
{
	public class EventSystem : IAttachable
	{
		private Bot bot;

		private Dictionary<Guid, CommandHandler> commandHandlers = new Dictionary<Guid, CommandHandler>();

		private EventSystemConfig config;

		public MessageFilter MessageFilter { get; private set; } = new MessageFilter();

		public EventSystem(EventSystemConfig config)
		{
			this.config = config;
		}

		public void AddCommandHandler(CommandHandler handler)
		{
			commandHandlers.Add(handler.GetType().GUID, handler);
		}

		public void AttachTo(Bot bot)
		{
			this.bot = bot;
			bot.Client.MessageCreate += OnMessageReceivedAsync;
		}

		public T GetCommandHandler<T>() where T : CommandHandler
		{
			 if(commandHandlers.TryGetValue(typeof(T).GUID, out var commandHandler))
			{
				return commandHandler as T;
			}
			return null;
		}

		public async Task OnMessageReceivedAsync(IDiscordMessage msg)
		{
			if (!await MessageFilter.IsAllowedAsync(msg))
			{
				return;
			}

			Task task = Task.Run(async () =>
			{
				var stopwatch = Stopwatch.StartNew();

				try
				{
					MessageContext context = new MessageContext();
					context.message = msg;
					context.eventSystem = this;

					foreach (var handler in commandHandlers.Values)
					{
						await handler.CheckAsync(context);
					}
				}
				catch (BotException botEx)
				{
					config.ErrorEmbedBuilder
						.SetDescription(Locale.GetString(msg.ChannelId, botEx.Resource))
						.ToEmbed()
						.QueueToChannel(await msg.GetChannelAsync());
				}
				catch (Exception ex)
				{
					var embed = config.ErrorEmbedBuilder
						.SetDescription("Something went wrong internally. This could be due to the very new system. Please report this issue to the miki developer. This message will only appear until the developer thinks the system is good enough to run without major issues.")
						.AddInlineField("Error code", ex.Message)
						.AddInlineField("Source", ex.Source)
						.ToEmbed();

					await bot.Client._apiClient.SendMessageAsync(
						msg.ChannelId,
						new MessageArgs
						{
							content = "discord invite url: https://discord.gg/veUGD9t",
							embed = embed
						});
				}

				stopwatch.Stop();
			});
		}
	}
}