using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miki.Framework.Utils;

namespace Miki.Framework.Hosting
{
    public class BotApplicationBuilder : IBotApplicationBuilder
    {
        private const string ApplicationServicesKey = "Miki.ApplicationServices";
        
        private readonly IList<Func<MessageDelegate, MessageDelegate>> components = new List<Func<MessageDelegate, MessageDelegate>>();

        public BotApplicationBuilder()
        {
            Properties = new Dictionary<string, object>();
        }
        
        public BotApplicationBuilder(IBotApplicationBuilder builder)
        {
            Properties = new CopyOnWriteDictionary<string, object>(builder.Properties, StringComparer.Ordinal);
        }

        public IServiceProvider ApplicationServices
        {
            get => GetProperty<IServiceProvider>(ApplicationServicesKey);
            set => SetProperty(ApplicationServicesKey, value);
        }

        public IDictionary<string, object> Properties { get; }

        public T GetProperty<T>(string key)
        {
            return Properties.TryGetValue(key, out var value) ? (T) value : default;
        }

        public void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }

        public IBotApplicationBuilder Use(Func<MessageDelegate, MessageDelegate> middleware)
        {
            components.Add(middleware);
            return this;
        }
        
        private static ValueTask InvokeAsync(IContext context)
        {
            return context.Executable?.ExecuteAsync(context) ?? default;
        }

        public MessageDelegate Build()
        {
            return components.Reverse().Aggregate((MessageDelegate) InvokeAsync, (current, component) => component(current));
        }

        public IBotApplicationBuilder New()
        {
            return new BotApplicationBuilder(this);
        }
    }
}