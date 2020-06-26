using System;
using System.Collections.Generic;
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
        
        public Dictionary<string, ParameterExpression> Variables { get; } = new Dictionary<string, ParameterExpression>();
        
        public List<Expression> Initializers { get; } = new List<Expression>();

        public Expression Context { get; }

        public Expression GetOrSet(string name, Func<Expression> factory)
        {
            if (Variables.TryGetValue(name, out var variable))
            {
                return variable;
            }
            
            var expression = factory();
            variable = Expression.Variable(expression.Type, name);
            Variables.Add(name, variable);
            Initializers.Add(Expression.Assign(variable, expression));
            return variable;
        }
		
        public Expression GetService(Type type)
        {
            return GetOrSet(type.Name, () => Expression.Call(GetServiceMethod.MakeGenericMethod(type), Context));
        }

        public Expression GetContext(Type type, string name)
        {
            return GetOrSet("Context" + name, () => Expression.Call(GetContextMethod.MakeGenericMethod(type), Context, Expression.Constant(name)));
        }
    }
}