using Miki.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Framework
{
    public interface IMutableContext : IContext
    {
        void SetExecutable(IExecutable exec);
        void SetContext<T>(string id, T value);
    }
}
