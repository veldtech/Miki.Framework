namespace Miki.Framework
{
    using System.Threading.Tasks;
    using Miki.Discord.Common;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Logging;

    /// <summary>
    /// Starting point of Miki.Framework. Extend this class to create a managed start flow.
    /// </summary>
    public abstract class MikiApp
	{
        [Obsolete("Avoid using the singleton pattern on Miki, instead pass the App to your classes")]
		public static MikiApp Instance { get; private set; }

		public IServiceProvider Services { get; protected set; }

        public IAsyncEventingExecutor<IDiscordMessage> Pipeline { get; private set; }

        /// <summary>
        /// Initializes the current instance.
        /// </summary>
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