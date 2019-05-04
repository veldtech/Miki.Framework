using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework
{
    public interface IExecutable
    {
        Task RunAsync(IContext context);
    }
}
