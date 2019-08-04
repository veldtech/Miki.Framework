using Microsoft.Extensions.DependencyInjection;
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