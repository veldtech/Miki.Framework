using Miki.Discord.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Framework.Events.Filters
{
	public class UserFilter : IFilter
	{
		public HashSet<ulong> Users { get; private set; } = new HashSet<ulong>();

		public Task<bool> FilterAsync(IDiscordMessage msg)
			=> Task.FromResult(Users.Contains(msg.Author.Id));
	}
}