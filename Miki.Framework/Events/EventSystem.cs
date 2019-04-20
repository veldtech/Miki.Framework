using Microsoft.Extensions.DependencyInjection;
using Miki.Discord;
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

        public void Subscribe(DiscordClient client)
        {
            client.MessageCreate += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(IDiscordMessage msg)
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
                foreach (var x in _messageTriggers)
                {
                    CommandContext commandContext = (await x.CheckTriggerAsync(context, msg)) as CommandContext;
                    if (commandContext == null)
                    {
                        continue;
                    }

                    var tasks = commandHandlers.Values.Select(y => y.CheckAsync(commandContext));
                    await Task.WhenAll(tasks)
                        .ContinueWith(async t =>
                        {
                            if (t.IsFaulted)
                            {
                                foreach (var ex in t.Exception.InnerExceptions)
                                {
                                    await OnError(ex, commandContext);
                                }
                            }
                        });

                    break;
                }
            }
        }
    }
}