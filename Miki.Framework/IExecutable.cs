using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework
{
    public interface IExecutableCommand : IExecutable
    {
        string GetIdentifier();
    }

    public interface IExecutable
    {
        Task RunAsync(IContext context);
    }
}
