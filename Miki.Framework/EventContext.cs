using Miki.Discord.Common;
using Miki.Framework.Events.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
	public class EventContext
	{
		public Args Arguments;

		public CommandHandler commandHandler;

		public IDiscordMessage message;

		public IDiscordUser Author => message.Author;

		public IDiscordChannel Channel { get; internal set; }

		public IDiscordGuild Guild { get; internal set; }

		public EventSystem EventSystem;

		public CommandSession CreateSession()
			=> new CommandSession() { ChannelId = Channel.Id, UserId = Author.Id };
	}
}
