using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Miki.Framework.Arguments;
using Miki.Framework.Commands.Pipelines;
using Miki.Framework.Hosting;

namespace Miki.Framework.Commands.Providers
{
    public class ArgumentParameterProvider : IParameterProvider
    {
        private static readonly MethodInfo TakeMethod = typeof(ArgumentPackContextExtensions)
            .GetMethods()
            .Single(m => m.Name == nameof(ArgumentPackContextExtensions.Take) && m.IsGenericMethod);
        
        public ArgumentParameterProvider(Type parameterType)
        {
            ParameterType = parameterType;
        }

        public Type ParameterType { get; }

        public Expression Provide(ParameterBuilder context)
        {
            var typedPack = context.GetContext(typeof(ITypedArgumentPack), ArgumentPackBuilder.ArgumentKey);
            
            return Expression.Call(TakeMethod.MakeGenericMethod(ParameterType), typedPack);
        }
    }
}