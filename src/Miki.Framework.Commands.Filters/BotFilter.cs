namespace Miki.Framework.Commands.Filters
{
    using System.Threading.Tasks;


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
