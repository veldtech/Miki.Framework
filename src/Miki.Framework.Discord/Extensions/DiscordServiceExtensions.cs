using System;
using Microsoft.Extensions.DependencyInjection;
using Miki.Discord.Common;
using Miki.Framework.Discord;
using Miki.Framework.Discord.Factories;
using Miki.Framework.Discord.Services;

// ReSharper disable once CheckNamespace
namespace Miki.Framework
{
    public static class DiscordServiceExtensions
    {
        public static IServiceCollection AddDiscord(this IServiceCollection services, Type factoryType, params object[] factoryArguments)
        {
            services.AddHostedService(provider =>
            {
                var factory = (IDiscordClientFactory) ActivatorUtilities.CreateInstance(provider, factoryType, factoryArguments);
                
                return ActivatorUtilities.CreateInstance<DiscordHostedService>(provider, factory);
            });
            
            return services;
        }
        
        public static IServiceCollection AddDiscord<TFactory>(this IServiceCollection services, params object[] factoryArguments)
            where TFactory : IDiscordClientFactory
        {
            return AddDiscord(services, typeof(TFactory), factoryArguments);
        }
        
        public static IServiceCollection AddDiscord(this IServiceCollection services, DiscordToken token)
        {
            AddDiscord<DefaultDiscordClientFactory>(services, token);
            return services;
        }
    }
}