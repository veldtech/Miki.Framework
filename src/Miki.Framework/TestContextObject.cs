using Miki.Discord.Common;

namespace Miki.Framework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Context object usable for testing. Avoids having to mock a lot of context related fetching.
    /// </summary>
    public class TestContextObject : IMutableContext
    {
        private readonly Dictionary<string, object> contextObjects = new Dictionary<string, object>();
        private readonly Dictionary<Type, object> serviceObjects = new Dictionary<Type, object>();

        /// <inheritdoc />
        public IDiscordMessage Message { get; set; }

        /// <inheritdoc />
        public IExecutable Executable { get; set; }

        /// <inheritdoc />
        public IServiceProvider Services { get; set; }

        /// <inheritdoc />
        public object GetContext(string id)
        {
            if(contextObjects.TryGetValue(id, out var value))
            {
                return value;
            }
            return default;
        }

        /// <inheritdoc />
        public object GetService(Type t)
        {
            if(serviceObjects.TryGetValue(t, out var value))
            {
                return value;
            }
            return default;
        }

        /// <inheritdoc />
        public void SetExecutable(IExecutable exec)
        {
            Executable = exec;
        }

        /// <inheritdoc />
        public void SetService(Type type, object value)
        {
            if(serviceObjects.ContainsKey(type))
            {
                serviceObjects[type] = value;
            }
            else
            {
                serviceObjects.Add(type, value);
            }
        }

        /// <inheritdoc />
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
    }
}