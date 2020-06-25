﻿using System;
using Microsoft.Extensions.DependencyInjection;
 using Miki.Cache;
 using Miki.Discord.Common;
using Miki.Framework.Hosting;

namespace Miki.Framework
{
    public static class ServiceCollectionExtensions
    {
        public static T GetOrCreateService<T>(this IServiceProvider provider)
        {
            return provider.GetService<T>() ?? ActivatorUtilities.CreateInstance<T>(provider);
        }
        
        public static IServiceCollection AddCacheClient<T>(this IServiceCollection services)
            where T : class, IExtendedCacheClient
        {
            services.AddSingleton<ICacheClient>(provider => provider.GetService<IExtendedCacheClient>());
            services.AddSingleton<IExtendedCacheClient, T>();
            return services;
        }
        
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
    }
}