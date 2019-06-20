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
        object GetStage(Type t);
        object GetService(Type t);
    }

    public class ContextObject : IMutableContext, IDisposable
    {
        private readonly Dictionary<string, object> _contextObjects;
        private readonly IServiceScope _scope;
        private readonly IServiceScope _stageScope;

        private IExecutable _executable;
      
        public IServiceProvider Services 
            => _scope.ServiceProvider;

        public IExecutable Executable 
            => _executable;

        public ContextObject(IServiceProvider p, IServiceProvider stages)
        {
            _contextObjects = new Dictionary<string, object>();
            _scope = p.CreateScope();
            _stageScope = stages.CreateScope();
        }

        public void Dispose()
        {
            _scope.Dispose();
            _stageScope.Dispose();
        }
        
        public T GetContext<T>(string id)
        {
            if (_contextObjects.TryGetValue(id, out var value))
            {
                return (T)value;
            }
            return default;
        }

        public T GetService<T>()
            => (T)GetService(typeof(T));

        public object GetService(Type t)
            => _scope.ServiceProvider
                .GetService(t);

        public object GetStage(Type t)
            => _stageScope.ServiceProvider
                .GetService(t);

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

        public void SetExecutable(IExecutable exec)
        {
            _executable = exec;
        }
    }

    public static class Extensions
    {
        public static T GetService<T>(this IContext c)
        {
            return (T)c.GetService(typeof(T));
        }
        public static T GetStage<T>(this IContext c)
        {
            return (T)c.GetStage(typeof(T));
        }
    }
}
