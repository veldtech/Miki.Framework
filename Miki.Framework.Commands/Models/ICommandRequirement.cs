using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public interface ICommandRequirement
    {
        Task<bool> CheckAsync(IContext e);
        Task OnCheckFail(IContext e);
    }
}
