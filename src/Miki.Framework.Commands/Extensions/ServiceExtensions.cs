using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

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
            var builder = new CommandTreeBuilder(services);
            configure(builder);
            services.AddSingleton(provider => builder.Build(provider));
            return services;
        }
    }
}