using Miki.Discord.Common;
using Miki.Framework.Events.Commands;
using Miki.Framework.Events.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public class EventSystem : IAttachable
	{
		internal DiscordBot bot;

		private Dictionary<Guid, CommandHandler> commandHandlers = new Dictionary<Guid, CommandHandler>();

		private readonly EventSystemConfig config;

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

		public void AttachTo(DiscordBot bot)
		{
			this.bot = bot;
			bot.Discord.MessageCreate += OnMessageReceivedAsync;
		}

		public T GetCommandHandler<T>() where T : CommandHandler
		{
			if (commandHandlers.TryGetValue(typeof(T).GUID, out var commandHandler))
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
				if (OnError != null)
				{
					await OnError(ex, context);
				}
			}
		}
	}
}