using System.Threading.Tasks;
using Miki.Discord.Common;

namespace Miki.Framework.Discord.Factories
{
    public interface IDiscordClientFactory
    {
        Task<IDiscordClient> CreateClientAsync();
    }
}