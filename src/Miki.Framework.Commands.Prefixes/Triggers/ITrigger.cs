namespace Miki.Framework.Commands.Prefixes.Triggers
{
    using System.Threading.Tasks;

    public interface ITrigger
	{
		Task<string> CheckTriggerAsync(IContext context);
	}
}
