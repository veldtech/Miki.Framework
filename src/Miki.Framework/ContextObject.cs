using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Miki.Framework
{
    /// <inheritdoc cref="IMutableContext" />
    public class ContextObject : IMutableContext, IDisposable
	{
		private readonly Dictionary<string, object> contextObjects;
		private readonly IServiceScope scope;

		/// <summary>
		/// Service collection of the current context.
		/// </summary>
        public IServiceProvider Services
			=> scope.ServiceProvider;

		/// <summary>
		/// Current set Executable.
		/// </summary>
		public IExecutable Executable { get; private set; }

		/// <summary>
		/// Creates a scoped context object
		/// </summary>
        public ContextObject(IServiceProvider p)
		{
			contextObjects = new Dictionary<string, object>();
			scope = p.CreateScope();
		}

        /// <inheritdoc/>
        public void Dispose()
		{
			scope.Dispose();
		}

        /// <inheritdoc/>
        public object GetContext(string id)
		{
			if(contextObjects.TryGetValue(id, out var value))
			{
				return value;
			}
			return default;
		}

        /// <inheritdoc/>
		public object GetService(Type t) => scope.ServiceProvider.GetService(t);

        /// <inheritdoc/>
        public void SetContext(string id, object value)
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

        /// <inheritdoc/>
        public void SetExecutable(IExecutable exec)
		{
			Executable = exec;
		}
	}
}
