using Microsoft.Extensions.DependencyInjection;
using System;

namespace Miki.Framework
{
	public class MikiAppBuilder
	{
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
        public MikiAppBuilder AddSingletonService<T>()
            where T : class
        {
            Services.AddSingleton<T>();
            return this;
        }
        public MikiAppBuilder AddSingletonService<T, TImpl>()
            where T : class
            where TImpl : class, T
        {
            Services.AddSingleton<T, TImpl>();
            return this;
        }
		public MikiAppBuilder AddSingletonService<T>(T value)
		{
			Services.AddSingleton(typeof(T), value);
			return this;
		}
        public MikiAppBuilder AddSingletonService(Type t, object value)
		{
			Services.AddSingleton(t, value);
			return this;
		}

		public MikiApp Build()
		{
			var app = new MikiApp();
			Services.AddSingleton(app);
			var services = Services.BuildServiceProvider();
			app.Services = services;
			return app;
		}
	}
}
