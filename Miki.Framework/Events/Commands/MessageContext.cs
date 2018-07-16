using Miki.Discord.Common;

namespace Miki.Framework.Events.Commands
{
    public struct MessageContext
    {
		public CommandHandler commandHandler;
		public EventSystem eventSystem;
		public IDiscordMessage message;
		public IDiscordChannel channel;
    }
}
