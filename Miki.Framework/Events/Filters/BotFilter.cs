using Miki.Discord.Common;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Filters
{
	public class BotFilter : IFilter
	{
		public async Task<bool> FilterAsync(IDiscordMessage msg)
		{
			return await Task.FromResult(msg.Author.IsBot);
		}
	}
}