using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Miki.Framework.Hosting
{
    public class ParameterBuilder
    {
        private static readonly MethodInfo GetServiceMethod = typeof(ContextExtensions)
            .GetMethods()
            .First(m => m.Name == nameof(ContextExtensions.GetService) && m.IsGenericMethod);
		
        private static readonly MethodInfo GetContextMethod = typeof(ContextExtensions)
            .GetMethods()
            .First(m => m.Name == nameof(ContextExtensions.GetContext) && m.IsGenericMethod);

        public ParameterBuilder(Expression context)
        {
            Context = context;
        }

        public Expression Context { get; }
		
        public Expression GetService(Type type)
        {
            return Expression.Call(GetServiceMethod.MakeGenericMethod(type), Context);
        }

        public Expression GetContext(Type type, string name)
        {
            return Expression.Call(GetContextMethod.MakeGenericMethod(type), Context, Expression.Constant(name));
        }
    }
}