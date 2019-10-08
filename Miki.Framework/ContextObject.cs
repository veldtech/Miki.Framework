using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Miki.Framework
{
	public interface IContext
	{
		IExecutable Executable { get; }
		IServiceProvider Services { get; }

		T GetContext<T>(string id);

		object GetService(Type t);
	}

	public class ContextObject : IMutableContext, IDisposable
	{
		private readonly Dictionary<string, object> contextObjects;
		private readonly IServiceScope scope;

        public IServiceProvider Services
			=> scope.ServiceProvider;

		public IExecutable Executable { get; private set; }

        public ContextObject(IServiceProvider p, IServiceProvider stages)
		{
			contextObjects = new Dictionary<string, object>();
			scope = p.CreateScope();
		}

		public void Dispose()
		{
			scope.Dispose();
		}

		public T GetContext<T>(string id)
		{
			if(contextObjects.TryGetValue(id, out var value))
			{
				return (T)value;
			}
			return default;
		}

		public T GetService<T>()
			=> (T)GetService(typeof(T));

		public object GetService(Type t)
			=> scope.ServiceProvider
				.GetService(t);

        public void SetContext<T>(string id, T value)
		{
			if(contextObjects.ContainsKey(id))
			{
				contextObjects[id] = value;
			}
			else
			{
				contextObjects.Add(id, value);
			}
		}

		public void SetExecutable(IExecutable exec)
		{
			Executable = exec;
		}
	}

	public static class Extensions
	{
		public static T GetService<T>(this IContext c)
		{
			return (T)c.GetService(typeof(T));
		}
    }
}
