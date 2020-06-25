using System.Threading.Tasks;
using Miki.Discord.Common;

namespace Miki.Framework.Hosting
{
    public interface IDiscordClientFactory
    {
        Task<IDiscordClient> CreateClientAsync();
    }
}