using System.Threading.Tasks;

namespace Miki.Framework.Commands.Prefixes.Triggers
{
    public interface ITrigger
	{
		Task<string> CheckTriggerAsync(IContext context);
	}
}
