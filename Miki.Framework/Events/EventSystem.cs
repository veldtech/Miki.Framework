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
using Miki.Discord.Common;
using Miki.Discord;
using Miki.Discord.Internal;
using Miki.Logging;

namespace Miki.Framework.Events
{
	public class EventSystem : IAttachable
	{
		private Bot bot;

		private Dictionary<Guid, CommandHandler> commandHandlers = new Dictionary<Guid, CommandHandler>();

		private EventSystemConfig config;

		public MessageFilter MessageFilter { get; private set; } = new MessageFilter();

		public Func<Exception, EventContext, Task> OnError;

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
				EventContext context = new EventContext();
				context.message = msg;
				context.EventSystem = this;

				try
				{
					foreach (var handler in commandHandlers.Values)
					{
						await handler.CheckAsync(context);
					}
				}
				catch (Exception ex)
				{
					if(OnError != null)
					{
						await OnError(ex, context);
					}
				}
			});
		}
	}
}