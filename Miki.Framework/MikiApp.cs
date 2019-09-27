using System.Collections.Generic;
using System.Threading.Tasks;
using Miki.Discord.Common;

namespace Miki.Framework
{
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public abstract class MikiApp
	{
		public static MikiApp Instance { get; private set; }

		public IServiceProvider Services { get; private set; }

        public IAsyncEventingExecutor<IDiscordMessage> Pipeline { get; private set; }

		protected MikiApp()
		{
			Instance = this;
        }

        public async Task StartAsync()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            Configure(serviceCollection);
            serviceCollection.AddSingleton(this);
            Services = serviceCollection.BuildServiceProvider();
            Pipeline = ConfigurePipeline(Services);
            var providers = ConfigureProviders(Services, Pipeline);

            await providers.StartAsync();
            await Task.Delay(-1); // Halts the thread.
        }

        public abstract ProviderCollection ConfigureProviders(
            IServiceProvider services,
            IAsyncEventingExecutor<IDiscordMessage> pipeline);

        public abstract IAsyncEventingExecutor<IDiscordMessage> ConfigurePipeline(IServiceProvider collection);

        public abstract void Configure(ServiceCollection collection);
    }
}