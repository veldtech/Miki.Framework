using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Miki.Discord.Common;

namespace Miki.Framework.Events.Filters
{
	public class UserFilter : IFilter
	{
		public HashSet<ulong> Users { get; private set; } = new HashSet<ulong>();

		public async Task<bool> FilterAsync(IDiscordMessage msg)
		{
			return Users.Contains(msg.Author.Id);
		}
	}
}
