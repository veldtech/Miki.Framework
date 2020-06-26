using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Miki.Framework.Hosting
{
    public class BotApplicationBuilderFactory : IBotApplicationBuilderFactory
    {
        private readonly Action<IBotApplicationBuilder> configure;
        private readonly IServiceProvider serviceProvider;
        private readonly IHostBuilder hostBuilder;

        public BotApplicationBuilderFactory(
            IServiceProvider serviceProvider,
            IHostBuilder hostBuilder = null,
            Action<IBotApplicationBuilder> configure = null)
        {
            this.configure = configure;
            this.serviceProvider = serviceProvider;
            this.hostBuilder = hostBuilder;
        }

        public IBotApplicationBuilder CreateBuilder()
        {
            var builder = new BotApplicationBuilder
            {
                ApplicationServices = serviceProvider
            };

            if (configure != null)
            {
                configure.Invoke(builder);
            }
            else if (hostBuilder != null
                     && hostBuilder.Properties.TryGetValue("UseStartup.StartupType", out var value)
                     && value is Type startupType)
            {
                InitializeStartup(serviceProvider, startupType, builder);
            }

            return builder;
        }

        /// <summary>
        /// Configure the pipeline through the ASP.NET Core startup class. 
        /// </summary>
        private static bool InitializeStartup(IServiceProvider provider, Type startupType, IBotApplicationBuilder builder)
        {
            var startupInterfaceType = Type.GetType("Microsoft.AspNetCore.Hosting.IStartup, Microsoft.AspNetCore.Hosting.Abstractions");
            object startup;

            if (startupInterfaceType != null)
            {
                startup = provider.GetService(startupInterfaceType);

                if (startup != null)
                {
                    startupType = startup.GetType();
                }
            }
            else
            {
                startup = null;
            }

            if (startup == null)
            {
                if (startupType != null)
                {
                    startup = ActivatorUtilities.CreateInstance(provider, startupType);
                }
                else
                {
                    return false;
                }
            }

            var configureMethod = startupType.GetMethod("ConfigureBot");

            if (configureMethod == null)
            {
                return false;
            }
            
            var parameterInfos = configureMethod.GetParameters();
            var parameters = new object[parameterInfos.Length];

            for (var i = 0; i < parameterInfos.Length; i++)
            {
                var parameterType = parameterInfos[i].ParameterType;

                if (parameterType == typeof(IBotApplicationBuilder))
                {
                    parameters[i] = builder;
                }
                else
                {
                    parameters[i] = provider.GetRequiredService(parameterType);
                }
            }

            configureMethod.Invoke(startup, parameters);

            return true;
        }
    }
}