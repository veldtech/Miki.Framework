using Microsoft.Extensions.DependencyInjection;
using Miki.Cache;
using Miki.Common;
using Miki.Discord;
using Miki.Discord.Common;
using System;

namespace Miki.Framework
{
	public class MikiApp
	{
		public static MikiApp Instance { get; internal set; }

        public IServiceProvider Services { get; }

		internal MikiApp(IServiceProvider provider)
		{
            Services = provider;
            Instance = this;
		}

        public T GetService<T>()
        {
            return Services.GetService<T>();
        }
    }
}