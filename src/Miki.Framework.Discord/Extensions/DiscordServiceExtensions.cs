using Microsoft.Extensions.DependencyInjection;
using Miki.Discord.Common;
using Miki.Framework.Discord;

// ReSharper disable once CheckNamespace
namespace Miki.Framework
{
    public static class DiscordServiceExtensions
    {
        public static IServiceCollection AddDiscord(this IServiceCollection services, DiscordToken token)
        {
            services.AddDiscord<DiscordClientFactory>(token);
            return services;
        }
    }
}