namespace Miki.Framework.Models
{
    using System.Threading.Tasks;

    public interface IMessage
	{
		Task DeleteAsync();

		Task<IChannel> GetChannelAsync();

		Task<IMessage> ModifyAsync(string content);
	}
}
