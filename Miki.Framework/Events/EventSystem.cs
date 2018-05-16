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
using Discord;
using Discord.WebSocket;
using System.Diagnostics;
using System.Threading;
using Miki.Framework.Events.Filters;
using Miki.Framework.Events.Commands;
using Miki.Framework.Exceptions;
using Miki.Framework.Extension;
using Miki.Framework.Languages;

namespace Miki.Framework.Events
{
	public class EventSystem : IAttachable
	{
		public event Func<Exception, CommandEvent, IMessage, int, Task> OnCommandDone;

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
			bot.Client.MessageReceived += OnMessageReceivedAsync;
		}

		public T GetCommandHandler<T>() where T : CommandHandler
		{
			 if(commandHandlers.TryGetValue(typeof(T).GUID, out var commandHandler))
			{
				return commandHandler as T;
			}
			return null;
		}

		public async Task OnMessageReceivedAsync(IMessage msg)
		{
			if (!await MessageFilter.Run(msg))
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
				catch(BotException botEx)
				{
					config.ErrorEmbedBuilder
						.WithDescription(Locale.GetString(msg.Channel.Id, botEx.Resource))
						.Build()
						.QueueToChannel(msg.Channel);
				}
				catch (Exception ex)
				{
					config.ErrorEmbedBuilder
						.WithDescription(ex.Message)
						.Build()
						.QueueToChannel(msg.Channel);
				}

				stopwatch.Stop();
			});
		}
	}
}