using Discord;

namespace Miki.Framework.Events.Commands
{
    public struct MessageContext
    {
		public CommandHandler commandHandler;
		public EventSystem eventSystem;
		public IMessage message;
    }
}
