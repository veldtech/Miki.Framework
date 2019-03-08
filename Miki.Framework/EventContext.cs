using Miki.Discord.Common;
using Miki.Framework.Arguments;
using Miki.Framework.Events.Commands;
using System;

namespace Miki.Framework.Events
{
	public class EventContext
	{
        public IDiscordUser Self { get; set; }

		public EventSystem EventSystem { get; set; }

        internal IServiceProvider Services { get; set; }

        public T GetService<T>()
        {
            return (T)Services.GetService(typeof(T));
        }
	}

    public class MessageContext : EventContext
    {
        public IDiscordUser Author => Message.Author;

        public IDiscordMessage Message { get; set; }

        public IDiscordTextChannel Channel { get; set; }

        public IDiscordGuild Guild { get; set; }

        public LocaleInstance Locale { get;set; }
    }

    public interface ICommandContext
    {
        IDiscordUser Self { get; set; }
        EventSystem EventSystem { get; set; }
        IDiscordUser Author { get; }
        IDiscordMessage Message { get; }
        IDiscordTextChannel Channel { get; }
        IDiscordGuild Guild { get; }
        LocaleInstance Locale { get; }
        ICommand Command { get; }
        CommandHandler CommandHandler { get; }
        PrefixTrigger Prefix { get; }
        string PrefixUsed { get; }
        ITypedArgumentPack Arguments { get; }

        T GetService<T>();
    }

    public class CommandContext : MessageContext, ICommandContext
    {
        public ICommand Command { get; set; }
        public CommandHandler CommandHandler { get; set; }
        public PrefixTrigger Prefix { get; set; }
        public string PrefixUsed { get; set; }
        public ITypedArgumentPack Arguments { get; set; }

        public static CommandContext FromMessageContext(MessageContext context)
        {
            return new CommandContext
            {
                Message = context.Message,
                Channel = context.Channel,
                Guild = context.Guild,
                Locale = context.Locale,
                Self = context.Self,
                EventSystem = context.EventSystem,
                Services = context.Services
            };
        }
    }
}