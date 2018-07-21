using Miki.Discord.Common;

namespace Miki.Framework.Events.Commands
{
    public struct MessageContext
    {
		public EventSystem eventSystem;
		public IDiscordMessage message;
    }
}
