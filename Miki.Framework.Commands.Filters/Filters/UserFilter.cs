using Miki.Discord.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Filters
{
	public class UserFilter : IFilter
	{
		public HashSet<long> Users { get; private set; } = new HashSet<long>();

		public ValueTask<bool> CheckAsync(IContext msg)
			=> new ValueTask<bool>(!Users.Contains((long)msg.GetMessage().Author.Id));
	}
}