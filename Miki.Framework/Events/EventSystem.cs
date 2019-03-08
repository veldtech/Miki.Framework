using Microsoft.Extensions.DependencyInjection;
using Miki.Discord.Common;
using Miki.Framework.Events.Commands;
using Miki.Framework.Events.Filters;
using Miki.Framework.Events.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
    public class EventSystem
    {
        private readonly Dictionary<Guid, CommandHandler> commandHandlers = new Dictionary<Guid, CommandHandler>();
        private readonly List<ITrigger<IDiscordMessage>> _messageTriggers = new List<ITrigger<IDiscordMessage>>();

        private IDiscordUser _selfUser;

        public MessageFilter MessageFilter { get; private set; } = new MessageFilter();

        public Func<Exception, EventContext, Task> OnError;

        public void AddMessageTrigger(ITrigger<IDiscordMessage> trigger)
        {
            _messageTriggers.Add(trigger);
        }

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

        public List<ITrigger<IDiscordMessage>> GetMessageTriggers()
        {
            return _messageTriggers;
        }

        public async Task OnMessageReceivedAsync(IDiscordMessage msg)
        {
            if (!await MessageFilter.IsAllowedAsync(msg))
            {
                return;
            }

            MessageContext context = new MessageContext();
            context.EventSystem = this;

            if (_selfUser == null)
            {
                _selfUser = await MikiApp.Instance.Discord.GetCurrentUserAsync();
            }
            context.Self = _selfUser;
            context.Message = msg;

            using (var scope = MikiApp.Instance.Services.CreateScope())
            {
                context.Services = scope.ServiceProvider;
                try
                {
                    foreach(var x in _messageTriggers)
                    {
                        var command = await x.CheckTrigger(context, msg);
                        if (command == null)
                        {
                            continue;
                        }

                        await Task.WhenAll(commandHandlers.Values.Select(y => y.CheckAsync(command as CommandContext)));
                        break;
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