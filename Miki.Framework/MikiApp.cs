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
		public static MikiApp Instance { get; private set; }

        public IServiceProvider Services { get; internal set; }

		internal MikiApp()
		{
            Instance = this;
		}

        public T GetService<T>()
        {
            return Services.GetService<T>();
        }
    }
}