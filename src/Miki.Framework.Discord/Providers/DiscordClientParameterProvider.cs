using System;
using System.Linq.Expressions;
using Miki.Discord.Common;
using Miki.Framework.Hosting;

namespace Miki.Framework.Discord.Providers
{
    public class DiscordClientParameterProvider : IParameterProvider
    {
        public Type ParameterType => typeof(IDiscordClient);
        
        public Expression Provide(ParameterBuilder context)
        {
            return context.GetContext(typeof(IDiscordClient), "DiscordClient");
        }
    }
}