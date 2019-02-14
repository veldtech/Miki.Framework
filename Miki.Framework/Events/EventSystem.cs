using Microsoft.Extensions.DependencyInjection;
using Miki.Discord.Common;
using Miki.Framework.Events.Commands;
using Miki.Framework.Events.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public class EventSystem
	{
		private readonly Dictionary<Guid, CommandHandler> commandHandlers = new Dictionary<Guid, CommandHandler>();

		public MessageFilter MessageFilter { get; private set; } = new MessageFilter();

		public Func<Exception, EventContext, Task> OnError;

		public void AddCommandHandler(CommandHandler handler)
		{
			commandHandlers.Add(handler.GetType().GUID, handler);
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

            using (var scope = MikiApp.Instance.Services.CreateScope())
            {
                context.Services = scope.ServiceProvider;

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
}