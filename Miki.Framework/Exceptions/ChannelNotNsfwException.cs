using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Exceptions
{
	public class ChannelNotNsfwException : BotException
	{
		public override string Resource => "error_channel_not_nsfw";

		public ChannelNotNsfwException() : base()
		{ }
	}
}
