namespace Miki.Framework.Commands
{
    using System.Threading.Tasks;

    public interface ICommandRequirement
	{
		Task<bool> CheckAsync(IContext e);
		Task OnCheckFail(IContext e);
	}
}
