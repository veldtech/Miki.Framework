using System;
using System.Linq.Expressions;

namespace Miki.Framework.Hosting
{
    public interface IParameterProvider
    {
        Type ParameterType { get; }

        Expression Provide(ParameterBuilder context);
    }
}