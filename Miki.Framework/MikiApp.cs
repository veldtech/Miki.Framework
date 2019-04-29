using Microsoft.Extensions.DependencyInjection;
using Miki.Cache;
using Miki.Common;
using Miki.Discord;
using Miki.Discord.Common;
using System;

namespace Miki.Framework
{
	public class MikiApp
	{
		public static MikiApp Instance { get; internal set; }

		public DiscordClient Discord { get; internal set; }

        public IServiceProvider Services { get; }

		internal MikiApp(ServiceProvider provider)
		{
            Services = provider;

            Discord = new DiscordClient(new DiscordClientConfigurations
            {
                ApiClient = provider.GetService<IApiClient>(),
                Gateway = provider.GetService<IGateway>(),
                CacheClient = provider.GetService<IExtendedCacheClient>()
            });

            Instance = this;
		}

        public T GetService<T>()
        {
            return Services.GetService<T>();
        }
    }
}