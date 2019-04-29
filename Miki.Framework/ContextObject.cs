using Microsoft.Extensions.DependencyInjection;
using Miki.Framework.Commands.Pipelines;
using System;
using System.Collections.Generic;

namespace Miki.Framework.Commands
{
    public interface IContext
    {
        IServiceProvider Services { get; }

        T GetContext<T>(string id);
        T GetService<T>();
    }

    public class ContextObject : IMutableContext, IDisposable
    {
        private Dictionary<string, object> _contextObjects = new Dictionary<string, object>();
        private readonly IServiceScope _scope;

        public IServiceProvider Services 
            => _scope.ServiceProvider;

        public ContextObject(IServiceProvider p)
        {
            _scope = p.CreateScope();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public T GetContext<T>(string id)
        {
            if (_contextObjects.TryGetValue(id, out var v))
            {
                return (T)v;
            }
            return default;
        }

        public T GetService<T>()
        {
            return _scope.ServiceProvider
                .GetService<T>();
        }

        public void SetContext<T>(string id, T value)
        {
            if (_contextObjects.ContainsKey(id))
            {
                _contextObjects[id] = value;
            }
            else
            {
                _contextObjects.Add(id, value);
            }
        }
    }
}
