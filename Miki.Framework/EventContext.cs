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
		public EventContext(IDiscordMessage message)
		{
			this.message = message;

			_channel = new ValueTask<IDiscordChannel>(message.GetChannelAsync());

			if (Channel is IDiscordGuildChannel gc)
			{
				_guild = new ValueTask<IDiscordGuild>(gc.GetGuildAsync());
			}
		}

		public Args Arguments;

		public CommandHandler commandHandler;

		public IDiscordMessage message;

		public IDiscordUser Author => message.Author;

		public IDiscordChannel Channel => _channel.Result;

		public IDiscordGuild Guild => _guild.Result;

		public EventSystem EventSystem;

		public CommandSession CreateSession()
			=> new CommandSession() { ChannelId = Channel.Id, UserId = Author.Id };

		private ValueTask<IDiscordChannel> _channel;
		private ValueTask<IDiscordGuild> _guild;
	}
}
