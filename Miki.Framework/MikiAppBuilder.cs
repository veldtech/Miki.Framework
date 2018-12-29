using Microsoft.Extensions.DependencyInjection;
using Miki.Common;
using System;

namespace Miki.Framework
{
    public class MikiAppBuilder
    {
        private readonly MikiApp _app;

        public IServiceCollection Services { get; }

        public MikiAppBuilder()
        {
            Services = new ServiceCollection();
        }

        public MikiAppBuilder AddSingletonService<T>(Func<IServiceProvider, T> factory)
            where T : class
        {
            Services.AddSingleton(factory);
            return this;
        }
        public MikiAppBuilder AddSingletonService<T>(T value)
        {
            Services.AddSingleton(typeof(T), value);
            return this;
        }
        
        public MikiApp Build()
        {
            return new MikiApp(Services.BuildServiceProvider());
        }
    }
}
