using Miki.Discord.Common;
using Miki.Framework.Arguments;
using System;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Arguments
{
    public class ArgumentPackBuilder : IPipelineStage
    {
        public ValueTask<bool> CheckAsync(IMutableContext e)
        {

        }
    }
}

namespace Miki.Framework.Commands
{
    public static class CommandPipelineExtensions
    {
        public static CommandPipelineBuilder WithArgumentPack(this CommandPipelineBuilder builder)
        {
            
        }
    }
}

namespace Miki.Framework
{
    public static class ContextExtensions
    {
        public static ITypedArgumentPack GetArguments(this IContext context)
        {
            return context.GetContext<ITypedArgumentPack>("framework-arguments");
        }
    }
}