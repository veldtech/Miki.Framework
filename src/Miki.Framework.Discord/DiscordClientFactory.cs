using System.Threading.Tasks;
using Miki.Cache;
using Miki.Discord;
using Miki.Discord.Common;
using Miki.Discord.Gateway;
using Miki.Discord.Rest;
using Miki.Framework.Hosting;

namespace Miki.Framework.Discord
{
    public class DiscordClientFactory : IDiscordClientFactory
    {
        private readonly IExtendedCacheClient cacheClient;
        private readonly DiscordToken token;

        public DiscordClientFactory(IExtendedCacheClient cacheClient, DiscordToken token)
        {
            this.cacheClient = cacheClient;
            this.token = token;
        }

        public Task<IDiscordClient> CreateClientAsync()
        {
            var gateway = new GatewayShard(new GatewayProperties
            {
                ShardCount = 1,
                ShardId = 0,
                Token = token.Token,
                AllowNonDispatchEvents = true,
                Intents = GatewayIntents.AllDefault | GatewayIntents.GuildMembers
            });

            var apiClient = new DiscordApiClient(token, cacheClient);
            
            var configuration = new DiscordClientConfigurations
            {
                Gateway = gateway,
                ApiClient = apiClient,
                CacheClient = cacheClient
            };

            return Task.FromResult<IDiscordClient>(new DiscordClient(configuration));
        }
    }
}