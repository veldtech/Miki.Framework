using System.Threading.Tasks;

namespace Miki.Framework.Commands.Filters.Filters
{
	/// <summary>
	/// Filters bot accounts
	/// </summary>
	public class BotFilter : IFilter
	{
		public Task<bool> CheckAsync(IContext e)
		{
			return Task.FromResult(!e.GetMessage().Author.IsBot);
		}
	}
}
