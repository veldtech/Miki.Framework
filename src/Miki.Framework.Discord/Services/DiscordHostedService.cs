using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Miki.Discord.Common;
using Miki.Framework.Discord.Factories;
using Miki.Framework.Hosting;
using Miki.Framework.Models;
using Miki.Logging;

namespace Miki.Framework.Discord.Services
{
    public class DiscordHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private readonly MessageDelegate invoke;
        private readonly IDiscordClientFactory discordClientFactory;
        private IDiscordClient discordClient;

        public DiscordHostedService(
            IBotApplicationBuilderFactory factory,
            IServiceProvider serviceProvider,
            IDiscordClientFactory discordClientFactory)
        {
            this.serviceProvider = serviceProvider;
            this.discordClientFactory = discordClientFactory;
            invoke = factory.CreateBuilder().Build();
        }

        private async Task HandleMessageAsync(IDiscordMessage message)
        {
            using var scope = serviceProvider.CreateScope();
            using var context = new ContextObject(scope.ServiceProvider, new DiscordMessage(message));
            
            try
            {
                await invoke(context);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (discordClient == null)
            {
                discordClient = await discordClientFactory.CreateClientAsync();
                discordClient.MessageCreate += HandleMessageAsync;
            }

            await discordClient.Gateway.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return discordClient?.Gateway.StopAsync() ?? Task.CompletedTask;
        }

        public void Dispose()
        {
            if (discordClient != null)
            {
                discordClient.MessageCreate -= HandleMessageAsync;
            }
        }
    }
}