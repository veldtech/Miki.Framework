using Miki.Common;
using Miki.Common.Interfaces;
using System.Threading.Tasks;

namespace Miki.Framework.Events
{
    public class GuildEvent : RuntimeCommandEvent
    {
        public ProcessServerCommand processCommand = async (e) =>
        {
            await (await e.GetDefaultChannelAsync()).SendMessageAsync("This server event has not been set up correctly.");
        };

        public GuildEvent()
        {
        }

        public async Task CheckAsync(IDiscordGuild e)
        {
            await Task.Run(() => processCommand(e));
        }
    }
}