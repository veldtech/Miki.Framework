using System.Threading.Tasks;

namespace Miki.Framework.Models
{
	public interface IMessage
	{
		Task DeleteAsync();

		Task<IChannel> GetChannelAsync();

		Task<IMessage> ModifyAsync(string content);
	}
}
