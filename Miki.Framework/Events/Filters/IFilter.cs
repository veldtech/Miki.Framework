using Miki.Discord.Common;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Filters
{
	public interface IFilter
	{
		Task<bool> FilterAsync(IDiscordMessage msg);
	}
}