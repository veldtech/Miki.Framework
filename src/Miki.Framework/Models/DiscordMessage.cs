using Miki.Discord.Common;

namespace Miki.Framework.Models
{
    public class DiscordMessage : IMessage
    {
        private readonly IDiscordMessage message;

        public DiscordMessage(IDiscordMessage message)
        {
            this.message = message;
        }

        public object InnerMessage => message;

        public string Content => message.Content;
    }
}