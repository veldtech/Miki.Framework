namespace Miki.Framework.Commands.Filters
{
    using System.Threading.Tasks;

    /// <summary>
	/// Filters bot accounts
	/// </summary>
	public class BotFilter : IFilter
	{
		public ValueTask<bool> CheckAsync(IContext e)
		{
			return new ValueTask<bool>(!e.GetMessage().Author.IsBot);
		}
	}
}
