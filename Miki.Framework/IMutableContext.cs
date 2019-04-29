using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework.Commands.Pipelines
{
    public interface IMutableContext : IContext
    {
        void SetContext<T>(string id, T value);
    }
}
