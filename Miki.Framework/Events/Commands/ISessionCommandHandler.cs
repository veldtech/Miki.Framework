using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Commands
{
    public interface ISessionCommandHandler : ICommandHandler
    {
		Task AddSession(CommandSession session, ICommandHandler handler);
	}
}
