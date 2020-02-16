using System.Collections.Generic;
using System.Threading.Tasks;
using Miki.Discord.Common;

namespace Miki.Framework
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Logging;

    public abstract class MikiApp
	{
		public static MikiApp Instance { get; private set; }

		public IServiceProvider Services { get; protected set; }

        public IAsyncEventingExecutor<IDiscordMessage> Pipeline { get; private set; }

		protected MikiApp()
		{
			Instance = this;
        }

        public async Task StartAsync()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            await ConfigureAsync(serviceCollection);
            serviceCollection.AddSingleton(this);
            Services = serviceCollection.BuildServiceProvider();

            if (Services.GetService<MessageWorker>() == null)
            {
                Log.Warning("No message worker setup. Messages will not send to data platforms.");
            }

            Pipeline = ConfigurePipeline(Services);
            var providers = ConfigureProviders(Services, Pipeline);

            await providers.StartAsync();
            await Task.Delay(-1); // Halts the thread.
        }

        public abstract ProviderCollection ConfigureProviders(
            IServiceProvider services,
            IAsyncEventingExecutor<IDiscordMessage> pipeline);

        public abstract IAsyncEventingExecutor<IDiscordMessage> ConfigurePipeline(IServiceProvider collection);

        public abstract Task ConfigureAsync(ServiceCollection collection);
    }
}