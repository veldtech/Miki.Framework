using Miki.Discord.Common;
using Miki.Framework.Events.Commands;
using System;

namespace Miki.Framework.Events
{
	public class EventContext
	{
		public Args Arguments;

		public CommandHandler commandHandler;

		public IDiscordMessage message;

		public IDiscordUser Author => message.Author;

		public IDiscordTextChannel Channel { get; internal set; }

		public IDiscordGuild Guild { get; internal set; }

        public IServiceProvider Services { get; internal set; }

		public EventSystem EventSystem;

		public PrefixInstance Prefix;

		public LocaleInstance Locale;
	}
}