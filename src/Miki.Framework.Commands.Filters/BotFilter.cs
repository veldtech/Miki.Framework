using System.Threading.Tasks;

namespace Miki.Framework.Commands.Filters
{
    /// <inheritdoc/>
	public class BotFilter : IFilter
	{
        /// <inheritdoc/>
		public ValueTask<bool> CheckAsync(IContext e)
		{
			return new ValueTask<bool>(!e.GetMessage().Author.IsBot);
		}
	}
}
