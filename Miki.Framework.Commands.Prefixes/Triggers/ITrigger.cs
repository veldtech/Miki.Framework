using System.Threading.Tasks;

namespace Miki.Framework.Events.Triggers
{
	public interface ITrigger<in T>
	{
		Task<string> CheckTriggerAsync(IContext context, T packet);
	}
}
