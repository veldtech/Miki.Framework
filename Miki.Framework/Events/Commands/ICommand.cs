using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Commands
{
    public interface ICommand
    {
        string Name { get; }

        Task<bool> IsEnabledAsync(EventContext context);

        Task ExecuteAsync(CommandContext e);
    }
}
