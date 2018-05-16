using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Miki.Framework.Events.Filters
{
	public class UserFilter : IFilter
	{
		public HashSet<ulong> Users { get; private set; } = new HashSet<ulong>();

		public async Task<bool> FilterAsync(IMessage msg)
		{
			return Users.Contains(msg.Author.Id);
		}
	}
}
