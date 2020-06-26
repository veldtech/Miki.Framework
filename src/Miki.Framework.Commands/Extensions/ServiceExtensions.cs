using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Miki.Framework.Arguments;
using Miki.Framework.Commands.Providers;
using Miki.Framework.Hosting;

namespace Miki.Framework.Commands
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection services, Assembly assembly)
        {
            return AddCommands(services, builder => builder.AddAssembly(assembly));
        }
        
        public static IServiceCollection AddCommands(this IServiceCollection services, Action<CommandTreeBuilder> configure)
        {
            var argumentParseProvider = new ArgumentParseProvider();
            services.AddSingleton(argumentParseProvider);

            foreach (var parser in argumentParseProvider.Parsers)
            {
                services.AddSingleton<IParameterProvider>(new ArgumentParameterProvider(parser.OutputType));
            }
            
            var builder = new CommandTreeBuilder(services);
            configure(builder);
            services.AddSingleton(provider => builder.Build(provider));
            return services;
        }
    }
}