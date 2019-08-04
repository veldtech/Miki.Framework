using System.Threading.Tasks;

namespace Miki.Framework.Events.Triggers
{
	public interface ITrigger<T>
	{
		Task<string> CheckTriggerAsync(IMutableContext context, T packet);
	}
}
