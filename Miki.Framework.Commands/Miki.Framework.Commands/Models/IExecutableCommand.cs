using Miki.Framework.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public interface IExecutableCommand
    {
        Task ExecuteAsync(CommandContext context);
    }
}
