using System;
using System.Threading.Tasks;

namespace Miki.Framework.Commands
{
    public abstract class CommandRequirementAttribute : Attribute, ICommandRequirement
	{
		public abstract Task<bool> CheckAsync(IContext e);
		public abstract Task OnCheckFail(IContext e);
	}
}
