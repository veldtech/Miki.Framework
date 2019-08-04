using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miki.Framework.Commands.Filters
{
	public class UserFilter : IFilter
	{
		public HashSet<long> Users { get; private set; } = new HashSet<long>();

		public Task<bool> CheckAsync(IContext msg)
			=> Task.FromResult(!Users.Contains((long)msg.GetMessage().Author.Id));
	}
}