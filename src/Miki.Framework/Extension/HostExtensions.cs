﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Miki.Framework.Hosting;

namespace Miki.Framework
{
    public static class HostExtensions
    {
        public static IHostBuilder ConfigureBot(
            this IHostBuilder hostBuilder,
            Action<IBotApplicationBuilder> configure = null)
        {
            hostBuilder.ConfigureServices((ctx, services) =>
            {
                services.AddSingleton<IBotApplicationBuilderFactory>(provider => new BotApplicationBuilderFactory(provider, hostBuilder, configure));
            });

            return hostBuilder;
        }
    }
}