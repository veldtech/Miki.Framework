using Microsoft.Extensions.DependencyInjection;
using Miki.Common;
using Miki.Framework.Arguments;
using System;

namespace Miki.Framework
{
    public class MikiAppBuilder
    {
        public IServiceCollection Services { get; }

        public MikiAppBuilder()
        {
            Services = new ServiceCollection();

            // TODO (Veld) : move to different location; or consider adding it through the App itself.
            ArgumentParseProvider parse = new ArgumentParseProvider();
            parse.SeedAssembly(typeof(ArgumentParseProvider).Assembly);
            AddSingletonService(parse);
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
