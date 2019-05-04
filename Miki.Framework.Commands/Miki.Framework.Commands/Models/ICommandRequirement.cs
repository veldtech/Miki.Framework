using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public interface ICommandRequirement
    {
        ValueTask<bool> CheckAsync(IContext e);
        Task OnCheckFail(IContext e);
    }
}
